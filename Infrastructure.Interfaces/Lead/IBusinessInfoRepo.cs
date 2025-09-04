using Core.Models.Auth;
using Core.ViewModels.Dto.Auth.Auth;
using Core.ViewModels.Dto.Lead;

namespace Infrastructure.Interfaces.Lead;

public interface IBusinessInfoRepo
{
    Task<IList<AspNetBusinessInfo>> GetAsync(UserInfoViewModel parameter);
    Task AddAsync(AspNetBusinessInfoDto businessInfoDto);
}
