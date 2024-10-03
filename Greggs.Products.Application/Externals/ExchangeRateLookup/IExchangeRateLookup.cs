namespace Greggs.Products.Application.Externals.ExchangeRateLookup;

public interface IExchangeRateLookup
{
    decimal GetExchangeRate(string currencyCode);
}