using Application.Interfaces.Auth;
using Core.Models.Auth;
using Infrastructure.Interfaces.Lead;

namespace Application.Services.Auth;

public class ServiceTypeService(IGenericRepo<AspNetServiceTypes> serviceType) : IServiceTypeService
{
    private readonly IGenericRepo<AspNetServiceTypes> _iServiceType = serviceType;
    public async Task<IList<AspNetServiceTypes>> GetAspNetServiceTypesAsync(dynamic parameter) => await _iServiceType.GetAllAsync(parameter);
    
}
