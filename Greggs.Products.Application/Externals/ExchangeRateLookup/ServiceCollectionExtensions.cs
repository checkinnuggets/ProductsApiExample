using Microsoft.Extensions.DependencyInjection;

namespace Greggs.Products.Application.Externals.ExchangeRateLookup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHardcodedExchangeLookup(this IServiceCollection services)
    {
        services.AddSingleton<IExchangeRateLookup, HardCodedExchangeRateLookup>();
        return services;
    }
}