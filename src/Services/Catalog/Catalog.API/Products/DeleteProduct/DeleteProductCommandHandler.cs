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

public class DeleteProductCommandHandler ( IDocumentSession session ) : ICommandHandler<DeleteProductCommand , DeleteProductResult>
{
    private readonly IDocumentSession _session = session;

    public async Task<DeleteProductResult> Handle ( DeleteProductCommand command , CancellationToken cancellationToken )
    {
        // Load product from database
        var product = await _session.LoadAsync<Product> ( command.Id , cancellationToken ) ?? throw new ProductNotFoundException ( command.Id );

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
