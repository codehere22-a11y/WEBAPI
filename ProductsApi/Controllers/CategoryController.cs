using Microsoft.AspNetCore.Mvc;
using ProductsApi.Data;
using ProductsApi.Models;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Domain; 
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
public async Task<IActionResult> Create(CreateCategoryDto dto)
{
    if (string.IsNullOrWhiteSpace(dto.Name))
        return BadRequest("Category name is required.");

    var categoryName = dto.Name.Trim();

    var existingCategory = await _context.Categories
        .FirstOrDefaultAsync(c => c.Name == categoryName);

    if (existingCategory is not null)
        return Conflict("Category already exists.");

    var category = new Category
    {
        Name = categoryName
    };

    _context.Categories.Add(category);
    await _context.SaveChangesAsync();

    return CreatedAtAction(
        nameof(GetById),
        new { id = category.Id },
        category);
}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _context.Categories.FindAsync(id);

        return category is null
            ? NotFound()
            : Ok(category);
    }
}