using Application.Interfaces.Auth;
using Core.Models.Auth;
using Infrustructure.Interfaces.Auth;

namespace Application.Services.Auth;

public class ServiceTypeService(IServiceTypeRepo serviceType) : IServiceTypeService
{
    private readonly IServiceTypeRepo _iServiceType = serviceType;
    public async Task<IList<AspNetServiceTypes>> GetAspNetServiceTypesAsync() => await _iServiceType.GetAspNetServiceTypesAsync();
    
}
