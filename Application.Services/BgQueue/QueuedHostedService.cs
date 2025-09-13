using Application.Interfaces.BgQueue;
using Common.Extentions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Application.Services.BgQueue;

public class QueuedHostedService(IServiceProvider serviceProvider, IBackgroundTaskQueue taskQueue) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IBackgroundTaskQueue _taskQueue = taskQueue;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await _taskQueue.DequeueAsync(stoppingToken);
            try
            {
                using var scope = _serviceProvider.CreateScope();
                await workItem(scope.ServiceProvider);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed QueuedHostedService Executing {At}", DateTime.Now.ToBangladeshTime());
            }
        }
    }
}
