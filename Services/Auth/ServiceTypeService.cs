using Application.Interfaces.Auth;
using Core.Models.Auth;
using Infrustructure.Interfaces.Auth;

namespace Application.Services.Auth
{
    public class ServiceTypeService(IServiceTypeRepo repo): IServiceTypeService
    {
        private readonly IServiceTypeRepo _repo = repo;
        public async Task<IList<AspNetServiceTypes>> GetAspNetServiceTypesAsync() => await _repo.GetAspNetServiceTypesAsync();
        
    }
}
