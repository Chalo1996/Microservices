namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommand ( string Name , List<string> Category , string Description , string ImageFile , decimal Price ) : ICommand<CreateProductResult>;
public record CreateProductResult ( Guid Id );
internal class CreateProductCommandHandler ( IDocumentSession session ) : ICommandHandler<CreateProductCommand , CreateProductResult>
{
    private readonly IDocumentSession _session = session;
    public async Task<CreateProductResult> Handle ( CreateProductCommand request , CancellationToken cancellationToken )
    {
        // Create product entity from command
        var product = new Product
        {
            Name = request.Name ,
            Category = request.Category ,
            Description = request.Description ,
            ImageFile = request.ImageFile ,
            Price = request.Price
        };

        // Save to database
        _session.Store ( product );
        await _session.SaveChangesAsync ( cancellationToken );
        // Return result
        var result = new CreateProductResult ( product.Id );
        return result;
    }
}
