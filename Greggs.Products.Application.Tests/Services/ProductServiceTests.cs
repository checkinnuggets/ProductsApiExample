using Greggs.Products.Application.Externals.ExchangeRateLookup;
using Greggs.Products.Application.Models;
using Greggs.Products.Application.Services;
using Greggs.Products.DataAccess;
using Greggs.Products.DataAccess.Models;

namespace Greggs.Products.Application.Tests.Services;

public class ProductServiceTests
{
    private readonly IExchangeRateLookup _mockExchangeRateLookup;
    private readonly IDataAccess<Product> _mockDataStore;

    private readonly IProductService _sut;

    public ProductServiceTests()
    {
        _mockExchangeRateLookup = Substitute.For<IExchangeRateLookup>();
        _mockDataStore = Substitute.For<IDataAccess<Product>>();

        _sut = new ProductService(_mockDataStore, _mockExchangeRateLookup);
    }

    [Fact]
    public void WhenPageStartIsNegativeThenResultIsFail()
    {
        // Arrange
        const int validPageSize = 2;
        const string validCurrency = "Any";

        // Act
        var result = _sut.GetProductList(-1, validPageSize, validCurrency);

        // Assert
        using var _ = new AssertionScope();
        result.IsFailed
            .Should().BeTrue();

        result.Errors.Select(x=>x.Message)
            .Should().BeEquivalentTo("First valid pageStart is 0.");
    }

    [Fact]
    public void WhenPageSizeIsGreaterThan10ThenResultIsFail()
    {
        // Arrange
        const int validPageStart = 0;
        const string validCurrency = "GBP";

        // Act
        var result = _sut.GetProductList(validPageStart, 11, validCurrency);


        // Assert
        using var _ = new AssertionScope();
        result.IsFailed
            .Should().BeTrue();

        result.Errors.Select(x => x.Message)
            .Should().BeEquivalentTo("Max page size is 10.");
    }

    [Fact]
    public void WhenCurrencyConversionErrorsThenResultIsFail()
    {
        // Arrange
        const int validPageStart = 0;
        const int validPageSize = 2;
        const string validCurrency = "GBP";

        _mockExchangeRateLookup.GetExchangeRate(validCurrency)
            .Returns(x => throw new ApplicationException("Error from currency converter."));
            

        // Act
        var result = _sut.GetProductList(validPageStart, validPageSize, validCurrency);


        // Assert
        using var _ = new AssertionScope();
        result.IsFailed
            .Should().BeTrue();

        result.Errors.Select(x => x.Message)
            .Should().BeEquivalentTo("Error from currency converter.");
    }

    [Fact]
    public void WhenCurrencyConversionSucceedsThenDataIsReturnedInTheRequestedCurrency()
    {
        // Arrange
        const int validPageStart = 0;
        const int validPageSize = 2;

        const string testCurrency = "XYZ";
        const decimal testExchangeRate = 2.0m;

            
        _mockDataStore.List(Arg.Any<int>(), Arg.Any<int>())
            .Returns(new[]
            {
                new Product { Name = "Product A", PriceInPounds = 1.0m },
                new Product { Name = "Product B", PriceInPounds = 0.5m },
                new Product { Name = "Product C", PriceInPounds = 2.0m },
            });

        _mockExchangeRateLookup.GetExchangeRate(testCurrency)
            .Returns(testExchangeRate);


        // Act
        var result = _sut.GetProductList(validPageStart, validPageSize, testCurrency);


        // Assert
        using var _ = new AssertionScope();
        result.IsSuccess
            .Should().BeTrue();

        result.Value
            .Should().BeEquivalentTo(new[]
            {
                // Prices converted using test exchange rate
                new ProductWithCurrency{ Name = "Product A", Price = 2.0m, PriceCurrency = testCurrency },
                new ProductWithCurrency{ Name = "Product B", Price = 1.0m, PriceCurrency = testCurrency },
                new ProductWithCurrency{ Name = "Product C", Price = 4.0m, PriceCurrency = testCurrency }
            });
    }
}