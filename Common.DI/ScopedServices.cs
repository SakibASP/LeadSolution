using Application.Interfaces.Auth;
using Application.Interfaces.Common;
using Application.Interfaces.Lead;
using Application.Interfaces.Menu;
using Application.Services.Auth;
using Application.Services.Common;
using Application.Services.Lead;
using Application.Services.Menu;
using Infrastructure.Interfaces.Auth;
using Infrastructure.Interfaces.Common;
using Infrastructure.Interfaces.Lead;
using Infrastructure.Interfaces.Menu;
using Infrastructure.Repositories.BusinessDomains.Auth;
using Infrastructure.Repositories.BusinessDomains.Common;
using Infrastructure.Repositories.BusinessDomains.Lead;
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
        services.AddScoped<IFormDetailService, FormDetailService>();
        services.AddScoped<IFormValueService, FormValueService>();
        services.AddScoped<IBusinessInfoService, BusinessInfoService>();
        services.AddScoped<IUtilityService, UtilityService>();
        services.AddScoped<IApiKeyService, ApiKeyService>();

        //Repositories
        services.AddScoped<IMenuRepo, MenuRepo>();
        services.AddScoped<IAdminRightsRepo, AdminRightsRepo>();
        services.AddScoped<IFormDetailRepo, FormDetailRepo>();
        services.AddScoped<IFormValueRepo, FormValueRepo>();
        services.AddScoped<IBusinessInfoRepo, BusinessInfoRepo>();
        services.AddScoped<IUtilityRepo, UtilityRepo>();
        services.AddScoped<IApiKeyRepo, ApiKeyRepo>();


        //Db Context and Generic Repository
        services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
        services.AddScoped<IDapperContext, DapperContext>();


        return services;
    }
}
