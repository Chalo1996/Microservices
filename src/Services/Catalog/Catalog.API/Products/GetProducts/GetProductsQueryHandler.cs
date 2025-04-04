
namespace Catalog.API.Products.GetProducts;

public record GetProductsQuery ( ) : IQuery<GetProductsResult>;

public record GetProductsResult ( IEnumerable<Product> Products );

internal class GetProductsQueryHandler ( IDocumentSession session , ILogger<GetProductsQueryHandler> logger ) : IQueryHandler<GetProductsQuery , GetProductsResult>
{
    private readonly IDocumentSession _session = session;
    private readonly ILogger<GetProductsQueryHandler> _logger = logger;
    public async Task<GetProductsResult> Handle ( GetProductsQuery query , CancellationToken cancellationToken )
    {
        _logger.LogInformation ( "Getting all products using query {@Query}." , query );
        var products = await _session.Query<Product> ( ).ToListAsync ( cancellationToken );

        return new GetProductsResult ( products );
    }
}
