using Application.Interfaces.BgQueue;
using Application.Services.BgQueue;
using Microsoft.Extensions.DependencyInjection;

namespace Common.DI;

public static class SingletonServices
{
    public static IServiceCollection AddLeadSingletonServices(this IServiceCollection services)
    {
        services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

        return services;
    }
}
