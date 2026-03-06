using Application.Abstractions.Interfaces.Storage;
using Application.Common.Settings.Storage;
using Web.Api.Infrastructure;
using Web.Api.Services;

namespace Web.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services , IConfiguration configuration)
    {
        services.AddSettings(configuration);
        services.AddApiServices();
        services.AddEndpointsApiExplorer();

        // REMARK: If you want to use Controllers, you'll need this.
        services.AddControllers();

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    private static IServiceCollection AddSettings(this IServiceCollection services , IConfiguration configuration)
    {
        services.AddOptions<StorageSettings>()
            .Bind(configuration.GetSection(StorageSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
    private static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IFileUrlResolver, FileUrlResolver>();
        return services;
    }
}
