using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Data;
using ProductsApi.Models;

namespace ProductsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _context.Products.ToListAsync();
        Console.WriteLine($"Rows pulled from DB: {products.Count}");
        var filtered = products;
        Console.WriteLine($"Filtered products count: {filtered.Count}");
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _context.Products.FindAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? category, [FromQuery] int? maxId)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrEmpty(category) && int.TryParse(category, out var categoryId))
            query = query.Where(p => p.CategoryId == categoryId);

        if (maxId.HasValue)
            query = query.Where(p => p.Id <= maxId.Value);

        var results = await query.ToListAsync();
        return Ok(results);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Name is required.");

        var newProduct = new Product { Name = dto.Name, CategoryId = dto.CategoryId };

        _context.Products.Add(newProduct);
        await _context.SaveChangesAsync();

        var resultDto = new ProductDto(newProduct.Id, newProduct.Name, newProduct.CategoryId);
        return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, resultDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateProductDto dto)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();

        product.Name = dto.Name;
        product.CategoryId = dto.CategoryId;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product is null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}