using Greggs.Products.Api.Models;
using Greggs.Products.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Greggs.Products.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Product[]), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public IActionResult Get(int pageStart = 0, int pageSize = 5, string currency = "GBP")
    {
        var result = _productService.GetProductList(pageStart, pageSize, currency);

        if (result.IsFailed)
        {
            var errorMessage = string.Join("; ", result.Errors);
            _logger.LogInformation(errorMessage);

            return Problem(statusCode: 400, detail: errorMessage);
        }

        var apiModels = result.Value.Select(p => new Product
        {
            Name = p.Name,
            Price = p.Price
        }).ToList();

        return Ok(apiModels);
    }
}