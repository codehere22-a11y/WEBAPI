namespace ProductsApi.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int CategoryId { get; set; }
     public Category Category { get; set; } = null!;
    public List<Tag> Tags { get; set; } = new();
    public Product() { }

    public Product(int id, string name, int categoryId)
    {
        Id = id;
        Name = name;
        CategoryId = categoryId;
    }
}