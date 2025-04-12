var builder = WebApplication.CreateBuilder ( args );

// Register services to the dependency injection container
builder.Services.AddMediatR ( config =>
{
    config.RegisterServicesFromAssembly ( Assembly.GetExecutingAssembly ( ) );
    config.AddOpenBehavior ( typeof ( LoggingBehaviors<,> ) );
    config.AddOpenBehavior ( typeof ( ValidationBehaviors<,> ) );
} );

builder.Services.AddValidatorsFromAssembly ( Assembly.GetExecutingAssembly ( ) );

builder.Services.AddCarter ( );
builder.Services.AddMarten ( opts =>
    opts.Connection ( builder.Configuration.GetConnectionString ( "CatalogConnection" ) )
).UseLightweightSessions ( );

if ( builder.Environment.IsDevelopment ( ) )
{
    builder.Services.InitializeMartenWith<CatalogInitialData> ( );
}

builder.Services.AddExceptionHandler<CustomExceptionHandler> ( );

builder.Services.AddHealthChecks ( )
    .AddNpgSql ( builder.Configuration.GetConnectionString ( "CatalogConnection" ) );

builder.AddServiceDefaults ( );
var app = builder.Build ( );

app.UseExceptionHandler ( opts => { } );
app.MapDefaultEndpoints ( );
app.MapCarter ( );
app.MapGet ( "/" , ( ) => "Hello World!" );
app.UseHealthChecks ( "/health" , new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
} );

app.Run ( );
