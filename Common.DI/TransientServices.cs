using Application.Interfaces.Common;
using Application.Services.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Common.DI;

public static class TransientServices
{
    public static IServiceCollection AddLeadTransientServices(this IServiceCollection services)
    {
        services.AddTransient<IEmailService, EmailService>();

        return services;
    }
}
