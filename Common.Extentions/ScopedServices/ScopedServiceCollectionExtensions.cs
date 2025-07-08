using Application.Interfaces.Auth;
using Application.Interfaces.Menu;
using Application.Services.Auth;
using Application.Services.Menu;
using Infrustructure.Interfaces.Auth;
using Infrustructure.Interfaces.Menu;
using Infrustructure.Repositories.Auth;
using Infrustructure.Repositories.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extentions.ScopedServices
{
    public static class ScopedServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedAppServices(this IServiceCollection services)
        {
            //Services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IServiceTypeService, ServiceTypeService>();
            services.AddScoped<IMenuService, MenuService>();

            //Repositories
            services.AddScoped<IServiceTypeRepo, ServiceTypeRepo>();
            services.AddScoped<IMenuRepo, MenuRepo>();

            return services;
        }
    }
}
