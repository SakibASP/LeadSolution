using Core.Models.Auth;
using Core.ViewModels.Dto.Lead;

namespace Infrastructure.Interfaces.Lead;

public interface IBusinessInfoRepo
{
    Task AddAsync(AspNetBusinessInfoDto businessInfoDto);
    Task UpdateAsync(AspNetBusinessInfo businessInfoDto);
}
