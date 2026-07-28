namespace ProductsApi.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
   
     public Category() { }  // needed for EF Core, which constructs objects then sets properties

    public Category(int id, string name)
    {
        Id = id;
        Name = name;
       
    }
}