namespace ProductsApi.Models;

public record ProductDto(int Id, string Name, int CategoryId);

public record CreateProductDto(string Name, int CategoryId);