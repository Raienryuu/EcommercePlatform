using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using ProductService.Controllers;
using ProductService.Models;
using ProductServiceTests.Fakes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ProductServiceTests;

public class ProductControllerTests
{

    private readonly ProductDbContextFakeBuilder _dbBuilder = new();
    private readonly ProductsController _cut;
    private readonly NullLogger<ProductsController> _nullLogger;

    public ProductControllerTests(){
        _nullLogger = new NullLogger<ProductsController>();
    }

    [Theory]
    [InlineData(1, typeof(OkObjectResult))]
    [InlineData(0, typeof(NoContentResult))]
    public async Task GetProduct_ProductId_Product(int productId, Type statusCodeResponse)
    {
        var _db = new ProductDbContextFakeBuilder()
        .WithProducts().Build();
        var _cut = new ProductsController(_nullLogger, _db);

        var result = await _cut.GetProduct(productId);

        Assert.Equal(statusCodeResponse, result.Result.GetType());
    }
}