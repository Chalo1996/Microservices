namespace Catalog.API.Products.DeleteProduct;

public record DeleteProductRequest ( Guid Id ) : ICommand<DeleteProductResponse>;
public record DeleteProductResponse ( bool IsSuccess );

public class DeleteProductEndpoint : ICarterModule
{
    public void AddRoutes ( IEndpointRouteBuilder app )
    {
        app.MapDelete ( "/products/{id}" , static async ( Guid id , ISender sender ) =>
        {
            var request = new DeleteProductRequest ( id );
            var command = request.Adapt<DeleteProductCommand> ( );
            var result = await sender.Send ( command );
            var response = result.Adapt<DeleteProductResponse> ( );
            return Results.Ok ( response );
        } )
        .WithName ( "DeleteProduct" )
        .Produces<DeleteProductResponse> ( StatusCodes.Status200OK )
        .ProducesProblem ( StatusCodes.Status400BadRequest )
        .ProducesProblem ( StatusCodes.Status404NotFound )
        .WithSummary ( "Delete Product" )
        .WithDescription ( "Delete product by id" );
    }
}