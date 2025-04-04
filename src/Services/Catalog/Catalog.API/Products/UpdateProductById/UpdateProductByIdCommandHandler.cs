namespace Catalog.API.Products.UpdateProduct;

public record UpdateProductCommand ( Guid Id , string Name , string Description , decimal Price , string ImageUrl , List<string> Category ) : ICommand<UpdateProductResult>;
public record UpdateProductResult ( bool IsSuccess );

public class UpdateProductByIdCommandHandler ( IDocumentSession session , ILogger<UpdateProductByIdCommandHandler> logger ) : ICommandHandler<UpdateProductCommand , UpdateProductResult>
{
    private readonly IDocumentSession _session = session;
    private readonly ILogger<UpdateProductByIdCommandHandler> _logger = logger;
    public async Task<UpdateProductResult> Handle ( UpdateProductCommand request , CancellationToken cancellationToken )
    {
        _logger.LogInformation ( "Updating product by id {@Id} using command {@Command}." , request.Id , request );
        // Load product from database
        var product = await _session.LoadAsync<Product> ( request.Id , cancellationToken );

        if ( product is null )
        {
            _logger.LogWarning ( "Product with id {@Id} not found." , request.Id );
            throw new ProductNotFoundException ( );
        }
        // Update product entity from command
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.ImageFile = request.ImageUrl;
        product.Category = product.Category;
        // Save to database
        _session.Update ( product );
        await _session.SaveChangesAsync ( cancellationToken );

        // Return result
        var result = new UpdateProductResult ( true );
        return result;
    }
}
