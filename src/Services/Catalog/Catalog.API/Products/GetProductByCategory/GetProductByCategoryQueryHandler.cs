namespace Catalog.API.Products.GetProductByCategory;

public record GetProductByCategoryQuery ( string Category ) : IQuery<GetProductByCategoryResult>;
public record GetProductByCategoryResult ( IEnumerable<Product> Products );

internal class GetProductByCategoryQueryHandler ( IDocumentSession session , ILogger<GetProductByCategoryQueryHandler> logger ) : IQueryHandler<GetProductByCategoryQuery , GetProductByCategoryResult>
{
    private readonly IDocumentSession _session = session;
    private readonly ILogger<GetProductByCategoryQueryHandler> _logger = logger;

    public async Task<GetProductByCategoryResult> Handle ( GetProductByCategoryQuery query , CancellationToken cancellationToken )
    {
        _logger.LogInformation ( "Getting products by category using query {@Query}." , query );
        var products = await _session.Query<Product> ( )
            .Where ( x => x.Category.Contains ( query.Category ) )
            .ToListAsync ( cancellationToken );
        return new GetProductByCategoryResult ( products );
    }
}
