namespace Catalog.API.Models;

public class Product
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<string> Category { get; set; } = [ ];
    public required string Description { get; set; }
    public required string ImageFile { get; set; }
    public decimal Price { get; set; }
}
