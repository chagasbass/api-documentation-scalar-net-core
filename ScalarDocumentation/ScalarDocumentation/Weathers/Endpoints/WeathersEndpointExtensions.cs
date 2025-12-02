namespace ScalarDocumentation.Weathers.Endpoints;

public static class WeathersEndpointExtensions
{
    static List<string> summaries = new List<string>
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};
    public static void InicializarEndpoints(this WebApplication app)
    {
        var apiVersionSet = VersionExtensions.RetornarVersaoDeEndpoints(app);

        app.MapGet("/v{version:apiVersion}/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Count)]
                ))
                .ToImmutableList();
            return forecast;
        })
           .Produces<IEnumerable<WeatherForecast>>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status401Unauthorized, typeof(ProblemDetails))
           .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
           .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
           .Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
           .WithName("WeathersV1")
           .WithTags("WeathersV1")
           .WithDescription(@"Endpoint resposável por exibir a previsão do tempo versão 1.")
           .WithSummary("Endpoint de Teste de aplicação V1")
           .WithApiVersionSet(apiVersionSet)
           .MapToApiVersion(new ApiVersion(1, 0));

        app.MapGet("/v{version:apiVersion}/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Count)]
                ))
                .ToImmutableList();
            return forecast;
        })
         .Produces<IEnumerable<WeatherForecast>>(StatusCodes.Status200OK)
         .Produces(StatusCodes.Status400BadRequest, typeof(ProblemDetails))
         .Produces(StatusCodes.Status404NotFound, typeof(ProblemDetails))
         .Produces(StatusCodes.Status500InternalServerError, typeof(ProblemDetails))
         .WithName("WeathersV2")
         .WithTags("WeathersV2")
         .WithDescription(@"Endpoint resposável por exibir a previsão do tempo versão 2.(Precisa de autenticação)")
         .WithSummary("Endpoint de Teste de aplicação V2")
         .WithApiVersionSet(apiVersionSet)
         .MapToApiVersion(new ApiVersion(2, 0))
         .RequireAuthorization();
    }
}
