namespace Catalog.API.Products.DeleteProduct;

public record class DeleteProductCommandRequestDTO
{
    public Guid Id { get; set; }
}

public record class DeleteProductCommandResponseDTO
{
    public bool IsSuccess { get; set; }
}

public record DeleteProductCommand : DeleteProductCommandRequestDTO, ICommand<DeleteProductResult>;
public record DeleteProductResult : DeleteProductCommandResponseDTO;

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator ( )
    {
        RuleFor ( x => x.Id )
            .NotEmpty ( )
            .WithMessage ( "Product id is required" );
    }
}

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
            throw new ProductNotFoundException ( command.Id );
        }

        // Delete product entity from database
        _session.Delete ( product );
        await _session.SaveChangesAsync ( cancellationToken );

        // Return result
        return new DeleteProductResult
        {
            IsSuccess = true
        };
    }
}
