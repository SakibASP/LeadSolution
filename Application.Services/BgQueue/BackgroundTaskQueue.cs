using Application.Interfaces.BgQueue;
using System.Threading.Channels;

namespace Application.Services.BgQueue;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<IServiceProvider, Task>> _queue;

    public BackgroundTaskQueue()
    {
        _queue = Channel.CreateUnbounded<Func<IServiceProvider, Task>>();
    }

    public void QueueBackgroundWorkItem(Func<IServiceProvider, Task> workItem)
    {
        if (!_queue.Writer.TryWrite(workItem))
        {
            throw new InvalidOperationException("Could not queue work item.");
        }
    }

    public async Task<Func<IServiceProvider, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);
        return workItem;
    }
}
