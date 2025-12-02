var builder = WebApplication.CreateBuilder(args);


//quantidade de versões de endpoints 
var versions = new List<string> { "v1", "v2" };

builder.Services.ConfigureDocumentationOptions(builder.Configuration);
builder.Services.AddDocumentationVersioningConfig(versions);
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

app.MapOpenApi();
app.UseScalarDocumentation();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

WeathersEndpointExtensions.InicializarEndpoints(app);

await app.RunAsync();
