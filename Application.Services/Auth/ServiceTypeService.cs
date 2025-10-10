using Application.Interfaces.Auth;
using Core.Models.Auth;
using Infrastructure.Interfaces.Common;

namespace Application.Services.Auth;

public class ServiceTypeService(IGenericRepo<AspNetServiceTypes> serviceType) : IServiceTypeService
{
    private readonly IGenericRepo<AspNetServiceTypes> _iServiceType = serviceType;
    public async Task<IList<AspNetServiceTypes>> GetAspNetServiceTypesAsync() => await _iServiceType.GetAllAsync();
    
}
