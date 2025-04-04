namespace Catalog.API.Products.DeleteProduct;

public record DeleteProductCommand ( Guid Id ) : ICommand<DeleteProductResult>;
public record DeleteProductResult ( bool IsSuccess );

public class DeleteProductCommandHandler ( IDocumentSession session , ILogger<DeleteProductCommandHandler> logger ) : ICommandHandler<DeleteProductCommand , DeleteProductResult>
{
    private readonly IDocumentSession _session = session;
    private readonly ILogger<DeleteProductCommandHandler> _logger = logger;

    public async Task<DeleteProductResult> Handle ( DeleteProductCommand command , CancellationToken cancellationToken )
    {
        _logger.LogInformation ( "Deleting product by id {@Id} using command {@Command}." , command.Id , command );
        // Load product from database
        var product = await _session.LoadAsync<Product> ( command.Id , cancellationToken );
        if ( product == null )
        {
            _logger.LogWarning ( "Product with id {@Id} not found." , command.Id );
            throw new ProductNotFoundException ( );
        }

        // Delete product entity from database
        _session.Delete ( product );
        await _session.SaveChangesAsync ( cancellationToken );

        // Return result
        return new DeleteProductResult ( true );
    }
}
