namespace ProductsApi.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Category Category { get; set; } = new Category();
     public Product() { }  // needed for EF Core, which constructs objects then sets properties

    public Product(int id, string name, Category category)
    {
        Id = id;
        Name = name;
        Category = category;
    }
}