using Takasbu.Models;
using Takasbu.Services;
using Microsoft.AspNetCore.Mvc;

namespace Takasbu.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _ProductsService;

    public ProductsController(ProductService ProductsService) =>
        _ProductsService = ProductsService;

    [HttpGet]
    public async Task<List<Product>> Get() =>
        await _ProductsService.GetAsync();

    [HttpGet]
    public async Task<ActionResult<Product>> Get(string id)
    {
        var Product = await _ProductsService.GetAsync(id);

        if (Product is null)
        {
            return NotFound();
        }

        return Product;
    }

    

    [HttpPost]
    public async Task<IActionResult> Post(Product newProduct)
    {
        await _ProductsService.CreateAsync(newProduct);

        return CreatedAtAction(nameof(Get), new { id = newProduct.Id }, newProduct);
    }

    [HttpPut]
    public async Task<IActionResult> Update(string id, Product updatedProduct)
    {
        var Product = await _ProductsService.GetAsync(id);

        if (Product is null)
        {
            return NotFound();
        }

        updatedProduct.Id = Product.Id;

        await _ProductsService.UpdateAsync(id, updatedProduct);

        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(string id)
    {
        var Product = await _ProductsService.GetAsync(id);

        if (Product is null)
        {
            return NotFound();
        }

        await _ProductsService.RemoveAsync(id);

        return NoContent();
    }
}