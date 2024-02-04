using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

public static class AuthorizationPolicy
{
    public const string Read = nameof(Read);
    public const string Write = nameof(Write);
}

public static class Scopes
{
    public const string Write = "write";
    public const string Read = "read";
}

public static class AuthorizationOptionsExtensions
{
    public static void AddAuthorizationPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(AuthorizationPolicy.Read,
            policy => policy
                .RequireClaim("scope", Scopes.Read));
        
        options.AddPolicy(AuthorizationPolicy.Write,
            policy => policy
                .RequireClaim("scope", Scopes.Write));
    }
}