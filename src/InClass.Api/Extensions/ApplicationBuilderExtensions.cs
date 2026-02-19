using Microsoft.AspNetCore.Authentication.JwtBearer;
using Scalar.AspNetCore;

namespace Web.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwaggerWithUi(this WebApplication app)
    {
        app.MapOpenApi();
        app.MapScalarApiReference(static options =>
        {
            options.AddPreferredSecuritySchemes(JwtBearerDefaults.AuthenticationScheme);
            options.AddHttpAuthentication(JwtBearerDefaults.AuthenticationScheme, _ => { });
            options.EnablePersistentAuthentication();
        });

        return app;
    }
}
