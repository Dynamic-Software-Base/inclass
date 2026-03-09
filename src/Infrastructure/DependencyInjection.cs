using System.Text;
using Application.Abstractions.Authentication;
using Application.Abstractions.Authorization;
using Application.Abstractions.Data;
using Application.Abstractions.Interfaces.Repositories;
using Application.Abstractions.Interfaces.Services;
using Infrastructure.Authentication;
using Infrastructure.Authentication.Services;
using Infrastructure.Authorization;
using Infrastructure.Database;
using Infrastructure.Geocoding;
using Infrastructure.Repositories;
using Infrastructure.Storage;
using Infrastructure.Time;
using Keycloak.AuthServices.Sdk;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SharedKernel;
using SharedKernel.Enums;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration) =>
        services
            .AddInfrastructureConfiguration(configuration)
            .AddServices()
            .AddDatabase(configuration)
            .AddHealthChecks(configuration)
            .AddAuthenticationInternal(configuration)
            .AddAuthorizationInternal()
            .AddStorageServices(configuration);

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IEnsureLocalUserService, EnsureLocalUser>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IMemberShipReposiory, MemberShipRepository>();
        services.AddScoped<ISchoolRepository, SchoolRepository>();
        services.AddHttpClient<IGeoCodingService, GoogleGeocodingService>();
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<ApplicationDbContext>(
            options => options
                .UseNpgsql(connectionString, npgsqlOptions =>
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Default))
                .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("Default")!);

        return services;
    }

    private static IServiceCollection AddInfrastructureConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<KeycloakSettings>()
            .Bind(configuration.GetSection(KeycloakSettings.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.Configure<GoogleGeocodingOptions>(
            configuration.GetSection(GoogleGeocodingOptions.SectionName));
        return services;
    }

    private static IServiceCollection AddAuthenticationInternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        KeycloakSettings keycloakSettings = configuration
                                                .GetSection(KeycloakSettings.SectionName)
                                                .Get<KeycloakSettings>()
                                            ?? throw new InvalidOperationException(
                                                $"Missing or invalid configuration section: {KeycloakSettings.SectionName}");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = keycloakSettings.Authority;
                options.Audience = keycloakSettings.ClientId;
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = keycloakSettings.Authority,
                    ValidAudience = "account", // Keycloak default audience
                    NameClaimType = "preferred_username",
                    RoleClaimType = "roles",
                    ClockSkew = TimeSpan.Zero // No tolerance for expired tokens
                };
                options.MapInboundClaims = false;
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"JWT Auth failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("JWT token validated successfully");
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();


        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IIdentityService, KeycloakIdentityService>();
        services.AddKeycloakAdminHttpClient(configuration);

        return services;
    }

    private static IServiceCollection AddAuthorizationInternal(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                SchoolPolicies.OwnerOnly,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new SchoolRoleRequirement(UserRole.SchoolOwner));
                });

            options.AddPolicy(
                SchoolPolicies.OwnerOrAdmin,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(
                        new SchoolRoleRequirement(UserRole.SchoolOwner, UserRole.SchoolAdministrator));
                });

            options.AddPolicy(
                SchoolPolicies.TeacherOnly,
                policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.Requirements.Add(new SchoolRoleRequirement(UserRole.Teacher));
                });
        });

        services.AddScoped<ISchoolAccessService, SchoolAccessService>();
        services.AddScoped<PermissionProvider>();

        services.AddScoped<IAuthorizationHandler, SchoolRoleAuthorizationHandler>();
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();

        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        return services;
    }
}
