using Microsoft.AspNetCore.Authorization;
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
[HttpPost("{productId}/tags/{tagId}")]
public async Task<IActionResult> AddTagToProduct(int productId, int tagId)
{
    var product = await _context.Products
        .Include(p => p.Tags)
        .FirstOrDefaultAsync(p => p.Id == productId);

    if (product is null) return NotFound("Product not found.");

    var tag = await _context.Tags.FindAsync(tagId);
    if (tag is null) return NotFound("Tag not found.");

    if (product.Tags.Any(t => t.Id == tagId))
        return Conflict("This product already has that tag.");

    product.Tags.Add(tag);
    await _context.SaveChangesAsync();

    return NoContent();
}

[HttpGet("{id}/with-tags")]
public async Task<IActionResult> GetByIdWithTags(int id)
{
    var product = await _context.Products
        .Include(p => p.Tags)
        .FirstOrDefaultAsync(p => p.Id == id);

    if (product is null) return NotFound();

    var result = new
    {
        product.Id,
        product.Name,
        Tags = product.Tags.Select(t => t.Name)
    };

    return Ok(result);
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
[HttpGet("with-categories-n-plus-one")]
public async Task<IActionResult> GetAllWithCategoriesNPlusOne()
{
    var products = await _context.Products.ToListAsync();  // query #1

    foreach (var p in products)
    {
        await _context.Entry(p).Reference(pr => pr.Category).LoadAsync();  // one query PER product
    }

    var result = products.Select(p => new { p.Id, p.Name, Category = p.Category?.Name });
    return Ok(result);
}

[HttpGet("with-categories-included")]
public async Task<IActionResult> GetAllWithCategoriesIncluded()
{
    var products = await _context.Products
        .Include(p => p.Category)
        .ToListAsync();  // ONE query, with a JOIN

    var result = products.Select(p => new { p.Id, p.Name, Category = p.Category?.Name });
    return Ok(result);
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
[Authorize(Roles = "Admin")]
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

[Authorize(Policy = "MinimumAge")]
[HttpGet("age-restricted-test")]
public IActionResult AgeRestrictedTest()
{
    return Ok("You're old enough to see this.");
}    
[Authorize(Roles = "Admin")]
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
[Authorize(Roles = "Admin")]
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