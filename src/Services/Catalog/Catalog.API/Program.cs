var builder = WebApplication.CreateBuilder ( args );

// Register services to the dependency injection container
// MediatR Configuration (Fixed)
builder.Services.AddMediatR ( config =>
{
    config.RegisterServicesFromAssembly ( Assembly.GetExecutingAssembly ( ) );
} );
builder.Services.AddTransient ( typeof ( IPipelineBehavior<,> ) , typeof ( ValidationBehaviors<,> ) );

builder.Services.AddValidatorsFromAssembly ( Assembly.GetExecutingAssembly ( ) );

builder.Services.AddCarter ( );
builder.Services.AddMarten ( opts =>
    opts.Connection ( builder.Configuration.GetConnectionString ( "CatalogConnection" ) )
).UseLightweightSessions ( );

builder.Services.AddExceptionHandler<CustomExceptionHandler> ( );

builder.AddServiceDefaults ( );
var app = builder.Build ( );

app.UseExceptionHandler ( opts => { } );
app.MapDefaultEndpoints ( );
app.MapCarter ( );
app.MapGet ( "/" , ( ) => "Hello World!" );

app.Run ( );
