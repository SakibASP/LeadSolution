using Application.Interfaces.Auth;
using Application.Services.Auth;
using Infrustructure.Interfaces.Auth;
using Infrustructure.Repositories.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extentions.ScopedServices
{
    public static class ScopedServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedAppServices(this IServiceCollection services)
        {
            // Scoped
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IServiceTypeService, ServiceTypeService>();
            services.AddScoped<IServiceTypeRepo, ServiceTypeRepo>();

            return services;
        }
    }
}
