namespace Catalog.API.Products.UpdateProductById;

public record UpdateProductCommandRequestDTO
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public List<string> Category { get; set; } = [ ];
    public required string Description { get; set; }
    public required string ImageFile { get; set; }
    public decimal Price { get; set; }
}

public record UpdateProductCommandResponseDTO
{
    public bool IsSuccess { get; set; }
}

public record UpdateProductCommand : UpdateProductCommandRequestDTO, ICommand<UpdateProductResult>;
public record UpdateProductResult : UpdateProductCommandResponseDTO;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator ( )
    {
        RuleFor ( x => x.Id )
            .NotEmpty ( )
            .WithMessage ( "Product id is required" );
        RuleFor ( x => x.Name )
            .NotEmpty ( )
            .WithMessage ( "Product name is required" )
            .Length ( 2 , 150 )
            .WithMessage ( "Product name must be between 2 and 100 characters" );
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
            throw new ProductNotFoundException ( request.Id );
        }

        // Update product entity from command
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.ImageFile = request.ImageFile;
        product.Category = product.Category;

        // Save to database
        _session.Update ( product );
        await _session.SaveChangesAsync ( cancellationToken );

        // Return result
        var result = new UpdateProductResult
        {
            IsSuccess = true
        };

        _logger.LogInformation ( "Product with id {@Id} updated successfully." , request.Id );

        return result;
    }
}
