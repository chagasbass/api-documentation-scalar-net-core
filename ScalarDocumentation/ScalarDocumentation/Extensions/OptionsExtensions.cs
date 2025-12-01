namespace ScalarDocumentation.Extensions;

public static class OptionsExtensions
{
    public static IServiceCollection ConfigureDocumentationOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DocumentationConfigurationOptions>(configuration.GetSection(DocumentationConfigurationOptions.BaseConfig));

        return services;
    }
}
