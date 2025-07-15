using Core.Models.Auth;

namespace Infrustructure.Interfaces.Auth;

public interface IServiceTypeRepo
{
    Task<IList<AspNetServiceTypes>> GetAspNetServiceTypesAsync();
}
