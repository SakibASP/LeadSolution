using Application.Interfaces.BgQueue;
using Application.Interfaces.Common;
using Common.Utils.Extentions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Common.DI;

public class EmailOnErrorSink(IFormatProvider? formatProvider,
    IBackgroundTaskQueue? taskQueue) : ILogEventSink
{
    private readonly IFormatProvider? _formatProvider = formatProvider;
    private readonly IBackgroundTaskQueue? _taskQueue = taskQueue;

    public void Emit(LogEvent logEvent)
    {
        // Only trigger for Error or Fatal
        if (logEvent.Level < LogEventLevel.Error)
            return;

        var message = logEvent.RenderMessage(_formatProvider);
        var exception = logEvent.Exception;

        // Queue background email
        _taskQueue?.QueueBackgroundWorkItem(async sp =>
        {
            var emailService = sp.GetRequiredService<IEmailService>();
            var config = sp.GetRequiredService<IConfiguration>();
            var exceptionEmail = config["ExceptionEmail"] ?? "sakibur.rahman.cse@gmail.com";

            var subject = $"🚨 Application Error: {logEvent.Level}";
            var body = $@"
                <h3>Serilog Error Notification</h3>
                <p><b>Level:</b> {logEvent.Level}</p>
                <p><b>Message:</b> {message}</p>
                <p><b>Exception:</b><pre>{exception}</pre></p>
                <p><b>Time:</b> {DateTime.Now.ToBangladeshTime()}</p>
            ";

            try
            {
                await emailService.SendMailAsync(exceptionEmail, subject, body);
            }
            catch (Exception emailEx)
            {
                Log.Error(emailEx, "Failed to send error email from Serilog Sink");
            }
        });
    }
}
