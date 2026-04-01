using System.Reflection;
using Application;
using Application.Abstractions.Authentication;
using Hangfire;
using HealthChecks.UI.Client;
using Infrastructure;
using Infrastructure.Database;
using Infrastructure.Outbox;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Web.Api;
using Web.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services.AddSwaggerGenWithAuth();
builder.Services.AddControllers();
builder.Services
    .AddApplication()
    .AddPresentation(builder.Configuration)
    .AddInfrastructure(builder.Configuration);


builder.Services.AddCors(options =>
{
    options.AddPolicy("BFF", policy =>
    {
        policy
            .WithOrigins(builder.Configuration["AllowedCorsOrigins"]!)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

WebApplication app = builder.Build();

app.UseHangfireDashboard("/hangfire");

using (IServiceScope scope = app.Services.CreateScope())
{
    IRecurringJobManager recurringJobManager = scope.ServiceProvider
        .GetRequiredService<IRecurringJobManager>();

    recurringJobManager.AddOrUpdate<ProcessOutboxMessagesJob>(
        "process-outbox-messages",
        job => job.ProcessAsync(CancellationToken.None),
        "*/30 * * * * *"); // every 30 seconds
}
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();
}

if (args.Contains("--migrate"))
{
    await MigrationRunner.RunMigrationAsync(app.Services);

    return;
}

if (args.Contains("--seed"))
{
    using IServiceScope scope = app.Services.CreateScope();
    DatabaseSeeder seeder = scope.ServiceProvider
        .GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAllAsync();
    return;
}
app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

app.UseCors("BFF");

app.UseAuthentication();

app.UseAuthorization();
app.Use(async (context, next) =>
{
    if (context.User?.Identity?.IsAuthenticated == true)
    {
        IEnsureLocalUserService provisioner = context.RequestServices.GetRequiredService<IEnsureLocalUserService>();
        await provisioner.EnsureLocalUserAsync(context.RequestAborted);
    }

    await next();
});
// REMARK: If you want to use Controllers, you'll need this.
app.MapControllers();

await app.RunAsync();

// REMARK: Required for functional and integration tests to work.
namespace InClass.Api
{
    public partial class Program;
}
