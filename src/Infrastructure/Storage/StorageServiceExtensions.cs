using Application.Abstractions.Interfaces.Repositories;
using Application.Abstractions.Interfaces.Storage;
using Application.Common.Settings.Storage;
using Infrastructure.Repositories;
using Infrastructure.Storage.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Storage;

public static class StorageServiceExtensions
{
    public static IServiceCollection AddStorageServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Bind settings
        services.Configure<StorageSettings>(
            configuration.GetSection(StorageSettings.SectionName));

        // Register validators
        services.AddScoped<IFileValidator, FileValidator>();

        // Register storage providers
        services.AddScoped<IStorageProvider, AzureBlobStorageProvider>();
        services.AddScoped<IStorageProvider, LocalFileStorageProvider>();

        // Register main storage service
        services.AddScoped<IStorageService, FileStorageService>();
        services.AddScoped<IStorageFileRepository, StoredFileRepository>();

        return services;
    }
}
