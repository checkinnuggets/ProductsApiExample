namespace Greggs.Products.Application.Externals.ExchangeRateLookup;

public class HardCodedExchangeRateLookup : IExchangeRateLookup
{
    // Added this to 'Externals' on the assumption of it being an external lookup,
    // rather than something from 'our' database.

    private readonly IDictionary<string, decimal> _conversionFromGbp = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
    {
        { "GBP", 1.0m },
        { "EUR", 1.1m }
    };

    public decimal GetExchangeRate(string currencyCode)
    {
        if (!_conversionFromGbp.TryGetValue(currencyCode, out var exchangeRate))
        {
            throw new ApplicationException($"Cannot convert currency values to to '{currencyCode}'.");
        }

        return exchangeRate;
    }
}