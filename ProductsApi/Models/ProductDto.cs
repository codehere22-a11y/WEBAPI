namespace ProductsApi.Models;

public record ProductDto(int Id, string Name, string Category);

public record CreateProductDto(string Name, string Category);