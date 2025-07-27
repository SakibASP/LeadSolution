using Application.Interfaces.Auth;
using Application.Interfaces.Lead;
using Application.Interfaces.Menu;
using Application.Services.Auth;
using Application.Services.Lead;
using Application.Services.Menu;
using Infrastructure.Interfaces.Lead;
using Infrastructure.Interfaces.Menu;
using Infrastructure.Repositories.BusinessDomains.Menu;
using Infrastructure.Repositories.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Common.DI;

public static class ScopedServices
{
    public static IServiceCollection AddScopedAppServices(this IServiceCollection services)
    {
        //Services
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IServiceTypeService, ServiceTypeService>();
        services.AddScoped<IMenuService, MenuService>();
        services.AddScoped<IAdminRightsService, AdminRightsService>();
        services.AddScoped<IDataTypeService, DataTypeService>();
        services.AddScoped<IBusinessInfoService, BusinessInfoService>();

        //Repositories
        services.AddScoped<IMenuRepo, MenuRepo>();
        services.AddScoped<IAdminRightsRepo, AdminRightsRepo>();
        services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));


        return services;
    }
}
