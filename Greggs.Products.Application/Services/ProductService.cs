using FluentResults;
using Greggs.Products.Application.Externals.ExchangeRateLookup;
using Greggs.Products.Application.Models;
using Greggs.Products.DataAccess;
using Greggs.Products.DataAccess.Models;

namespace Greggs.Products.Application.Services;

public interface IProductService
{
    Result<IEnumerable<ProductWithCurrency>> GetProductList(int pageStart, int pageSize, string currency);
}

public class ProductService : IProductService
{
    private readonly IDataAccess<Product> _dataAccess;
    private readonly IExchangeRateLookup _exchangeRateLookup;

    public ProductService(IDataAccess<Product> dataAccess, IExchangeRateLookup exchangeRateLookup)
    {
        _dataAccess = dataAccess;
        _exchangeRateLookup = exchangeRateLookup;
    }

    public Result<IEnumerable<ProductWithCurrency>> GetProductList(int pageStart, int pageSize, string currency)
    {
        if (pageStart < 0)
            return Result.Fail("First valid pageStart is 0.");

        if (pageSize < 1 || pageSize > 10)
            return Result.Fail("Max page size is 10.");

        var result = new List<ProductWithCurrency>();

        try
        {
            var exchangeRate = _exchangeRateLookup.GetExchangeRate(currency);

            var databaseProducts = _dataAccess.List(pageStart, pageSize);
            var productsInCurrency = databaseProducts.Select(p => new ProductWithCurrency
            {
                Name = p.Name, 
                PriceCurrency = currency, 
                Price = p.PriceInPounds * exchangeRate
            });

            result.AddRange(productsInCurrency);
        }
        catch (ApplicationException aex)
        {
            return Result.Fail(new[] { aex.Message });
        }

        return Result.Ok(result.AsEnumerable());
    }
}