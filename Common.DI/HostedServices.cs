using Application.Services.BgQueue;
using Microsoft.Extensions.DependencyInjection;

namespace Common.DI;

public static class HostedServices
{
    public static IServiceCollection AddLeadHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<QueuedHostedService>();

        return services;
    }
}
