﻿namespace Catalog.API.Products.GetProducts;

public record GetProductsQuery ( int? PageNumber = 1 , int? PageSize = 10 ) : IQuery<GetProductsResult>;

public record GetProductsResult ( IEnumerable<Product> Products );

internal class GetProductsQueryHandler ( IDocumentSession session ) : IQueryHandler<GetProductsQuery , GetProductsResult>
{
    private readonly IDocumentSession _session = session;
    public async Task<GetProductsResult> Handle ( GetProductsQuery query , CancellationToken cancellationToken )
    {
        var products = await _session.Query<Product> ( ).ToPagedListAsync ( query.PageNumber ?? 1 , query.PageSize ?? 10 , token: cancellationToken );

        return new GetProductsResult ( products );
    }
}
