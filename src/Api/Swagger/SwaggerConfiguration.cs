using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Api.Swagger;

public static class SwaggerConfiguration
{
    public const string User = nameof(User);
    public const string Admin = nameof(Admin);

    public static void AddCustomSwaggerGenOptions(this SwaggerGenOptions options)
    {
        options.SwaggerDoc(User,
            new OpenApiInfo { Title = $"{User} Endpoints", Version = "v1" });
        options.SwaggerDoc(Admin,
            new OpenApiInfo { Title = $"{Admin} Endpoints", Version = "v1" });

        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter the JWT-token. Bearer will be prepended.",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new List<string>()
            }
        });
    }

    public static void AddCustomSwaggerUIOptions(this SwaggerUIOptions options, bool isDevelopment)
    {
        if (isDevelopment)
        {
            options.EnableTryItOutByDefault();
            options.EnablePersistAuthorization();
        }

        options.SwaggerEndpoint($"./{User}/swagger.json", $"{User} Docs");
        options.SwaggerEndpoint($"./{Admin}/swagger.json", $"{Admin} Docs");
    }
}