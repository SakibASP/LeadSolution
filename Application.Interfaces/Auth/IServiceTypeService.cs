using Core.Models.Auth;

namespace Application.Interfaces.Auth;

public interface IServiceTypeService
{
    Task<IList<AspNetServiceTypes>> GetAspNetServiceTypesAsync(dynamic parameter);
}
