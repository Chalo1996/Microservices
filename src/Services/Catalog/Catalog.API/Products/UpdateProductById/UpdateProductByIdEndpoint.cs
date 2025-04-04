namespace Catalog.API.Products.UpdateProduct;

public record UpdateProductRequest ( Guid Id , string Name , string Description , decimal Price , string ImageUrl , List<string> Category ) : ICommand<UpdateProductResponse>;
public record UpdateProductResponse ( bool IsSuccess );

public class UpdateProductByIdEndpoint : ICarterModule
{
    public void AddRoutes ( IEndpointRouteBuilder app )
    {
        app.MapPut ( "/products" , static async ( UpdateProductRequest request , ISender sender ) =>
        {
            // Adapt the request to the command using Mapster
            var command = request.Adapt<UpdateProductCommand> ( );
            var result = await sender.Send ( command );
            var response = result.Adapt<UpdateProductResponse> ( );
            return Results.Ok ( response );
        } )
        .WithName ( "UpdateProductById" )
        .Produces<UpdateProductResponse> ( StatusCodes.Status200OK )
        .ProducesProblem ( StatusCodes.Status400BadRequest )
        .ProducesProblem ( StatusCodes.Status404NotFound )
        .WithSummary ( "Update Product By Id" )
        .WithDescription ( "Update product by id" );
    }
}
