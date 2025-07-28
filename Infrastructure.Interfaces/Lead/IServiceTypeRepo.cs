using Core.Models.Auth;

namespace Infrastructure.Interfaces.Lead;

public interface IServiceTypeRepo
{
    Task<IList<AspNetServiceTypes>> GetAllAsync();
}
