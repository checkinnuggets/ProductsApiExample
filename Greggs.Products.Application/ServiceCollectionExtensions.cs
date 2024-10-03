using Greggs.Products.Application.Externals.ExchangeRateLookup;
using Microsoft.Extensions.DependencyInjection;
using Greggs.Products.Application.Services;
using Greggs.Products.DataAccess;
using Greggs.Products.DataAccess.Models;

namespace Greggs.Products.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddSingleton<IDataAccess<Product>, ProductAccess>();
        services.AddHardcodedExchangeLookup();

        services.AddTransient<IProductService, ProductService>();

        return services;
    }
}