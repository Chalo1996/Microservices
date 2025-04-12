using Bogus;

namespace Catalog.API.Data;

public class CatalogInitialData : IInitialData
{
    public async Task Populate ( IDocumentStore store , CancellationToken cancellation )
    {
        using var session = store.LightweightSession ( );

        if ( await session.Query<Product> ( ).AnyAsync ( token: cancellation ) )
            return;

        session.Store<Product> ( GetPreconfiguredProducts ( ) );

        await session.SaveChangesAsync ( cancellation );
    }

    private static IEnumerable<Product> GetPreconfiguredProducts ( )
    {
        var categories = new List<List<string>>
        {
            new() { "Electronics", "Computers" },
            new() { "Electronics", "Phones" },
            new() { "Home", "Furniture" },
            new() { "Home", "Kitchen" },
            new() { "Clothing", "Men" },
            new() { "Clothing", "Women" },
            new() { "Sports", "Outdoor" },
            new() { "Books", "Fiction" },
            new() { "Books", "Non-Fiction" },
            new() { "Toys", "Games" }
        };

        var productFaker = new Faker<Product> ( )
            .RuleFor ( p => p.Id , f => Guid.NewGuid ( ) )
            .RuleFor ( p => p.Name , f => f.Commerce.ProductName ( ) )
            .RuleFor ( p => p.Category , f => f.PickRandom ( categories ) )
            .RuleFor ( p => p.Description , f => f.Commerce.ProductDescription ( ) )
            .RuleFor ( p => p.ImageFile , f => $"product-{f.Random.Int ( 1 , 10 )}.jpg" )
            .RuleFor ( p => p.Price , f => f.Random.Decimal ( 10 , 1000 ) );

        return productFaker.Generate ( 100 );
    }
}