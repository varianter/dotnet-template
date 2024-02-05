using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddApplicaton(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMediatR(cfg =>
            cfg
                .RegisterServicesFromAssemblyContaining(typeof(DependencyInjection))
                .AddOpenBehavior(typeof(LoggingBehavior<,>))
        );

        return builder;
    }
}