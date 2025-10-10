using Core.Models.Lead;
using Core.ViewModels.Response;

namespace Application.Interfaces.Lead;

public interface IDataTypeService
{
    Task<ApiResponse<IList<DataTypes>>> GetAllAsync();
    Task<ApiResponse<DataTypes>> GetByIdAsync(int id);
    Task<ApiResponse<dynamic>> AddAsync(DataTypes dataTypes);
    Task<ApiResponse<dynamic>> UpdateAsync(DataTypes dataTypes);
    Task<ApiResponse<dynamic>> RemoveAsync(int id);
}
