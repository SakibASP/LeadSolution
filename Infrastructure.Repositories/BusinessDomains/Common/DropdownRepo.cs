using Common.Utils.Constant;
using Core.ViewModels.Request.Common;
using Dapper;
using Infrastructure.Interfaces.Common;
using System.Data;

namespace Infrastructure.Repositories.BusinessDomains.Common;

public class DropdownRepo(IDapperContext dapper) : IDropdownRepo
{
    private readonly IDapperContext _dapper = dapper;

    public async Task<IList<T>> GetDropdownListAsync<T>(DropdownRequest request)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", request.Id);
        parameters.Add("@Param1", request.Param1);
        parameters.Add("@Param2", request.Param2);
        parameters.Add("@Param3", request.Param3);
        parameters.Add("@Param4", request.Param4);

        using var connection = _dapper.CreateConnection();
        await connection.OpenAsync();
        var result = await connection.QueryAsync<T>(
            Sp.usp_GetDropdownList,
            parameters,
            commandType: CommandType.StoredProcedure);

        return [.. result];
    }
}
