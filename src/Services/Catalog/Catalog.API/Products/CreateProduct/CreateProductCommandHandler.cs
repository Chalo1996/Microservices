namespace Catalog.API.Products.CreateProduct;

public record CreateProductCommandRequestDTO
{
    public required string Name { get; set; }
    public List<string> Category { get; set; } = [ ];
    public required string Description { get; set; }
    public required string ImageFile { get; set; }
    public decimal Price { get; set; }
}

public record CreateProductCommandResponseDTO
{
    public Guid Id { get; set; }
}

public record CreateProductCommand : CreateProductCommandRequestDTO, ICommand<CreateProductResult>;
public record CreateProductResult : CreateProductCommandResponseDTO;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator ( )
    {
        RuleFor ( x => x.Name )
            .NotEmpty ( )
            .WithMessage ( "Product name is required" );
        RuleFor ( x => x.Category )
             .NotEmpty ( )
             .WithMessage ( "Product category is required" )
             .Must ( x => x.Count > 0 )
             .WithMessage ( "Product category must have at least one item" );
        RuleFor ( x => x.Description )
            .NotEmpty ( )
            .WithMessage ( "Product description is required" )
            .Length ( 2 , 500 )
            .WithMessage ( "Product description must be between 2 and 500 characters" );
        RuleFor ( x => x.ImageFile )
            .NotEmpty ( )
            .WithMessage ( "Product image is required" );
        RuleFor ( x => x.Price )
            .GreaterThan ( 0 )
            .WithMessage ( "Product price must be greater than 0" );
    }
}

internal class CreateProductCommandHandler ( IDocumentSession session ) : ICommandHandler<CreateProductCommand , CreateProductResult>
{
    private readonly IDocumentSession _session = session;
    public async Task<CreateProductResult> Handle ( CreateProductCommand command , CancellationToken cancellationToken )
    {

        // Create product entity from command
        var product = new Product
        {
            Name = command.Name ,
            Category = command.Category ,
            Description = command.Description ,
            ImageFile = command.ImageFile ,
            Price = command.Price
        };

        // Save to database
        _session.Store ( product );
        await _session.SaveChangesAsync ( cancellationToken );
        // Return result
        var result = new CreateProductResult
        {
            Id = product.Id
        };

        return result;
    }
}
