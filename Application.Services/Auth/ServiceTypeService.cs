using Application.Interfaces.Auth;
using Core.Models.Auth;
using Infrustructure.Interfaces.Auth;

namespace Application.Services.Auth;

public class ServiceTypeService(IServiceTypeRepo repo): IServiceTypeService
{
    private readonly IServiceTypeRepo _iRepo = repo;
    public async Task<IList<AspNetServiceTypes>> GetAspNetServiceTypesAsync() => await _iRepo.GetAspNetServiceTypesAsync();
    
}
