var builder = WebApplication.CreateBuilder(args);

// Register services to the dependency injection container
builder.Services.AddCarter();
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});
builder.Services.AddMarten(opts =>
    opts.Connection(builder.Configuration.GetConnectionString("CatalogConnection"))
).UseLightweightSessions();

builder.AddServiceDefaults();
var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/" , () => "Hello World!");

// Configure the HTTP request pipeline
app.MapCarter();

app.Run();
