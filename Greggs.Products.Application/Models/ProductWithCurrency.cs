namespace Greggs.Products.Application.Models;

public class ProductWithCurrency
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string PriceCurrency { get; set; }
}