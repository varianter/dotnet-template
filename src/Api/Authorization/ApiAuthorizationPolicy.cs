using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

public static class AuthorizationPolicy
{
    public const string Admin = nameof(Admin);
    public const string User = nameof(User);
    public const string Write = nameof(Write);
}

public static class Scopes
{
    public const string Write = "write";
    public const string Read = "read";
    public const string Admin = "admin";
}

public static class AuthorizationOptionsExtensions
{
    public static void AddAuthorizationPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(AuthorizationPolicy.User,
            policy => policy
                .RequireClaim("scope", Scopes.Read, Scopes.Admin));

        options.AddPolicy(AuthorizationPolicy.Write,
            policy => policy
                .RequireClaim("scope", Scopes.Write, Scopes.Admin));
        
        options.AddPolicy(AuthorizationPolicy.Admin,
            policy => policy
                .RequireClaim("scope", Scopes.Admin));
    }
}