using Common.Utils.Constant;
using Common.Utils.Helper;
using Core.ViewModels.Dto.Common;
using Core.ViewModels.Request.Common;
using Infrastructure.Interfaces.Common;
using Infrastructure.Repositories.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.BusinessDomains.Common;

public class DropdownRepo(LeadContext context) : IDropdownRepo
{
    private readonly LeadContext _context = context;
    public async Task<IList<DropdownDto>> GetDropdownListAsync(DropdownRequest request)
    {
        var parameters = GetParameter(request);
        var extendedSp = SpNameHelper.GetSpName(Sp.usp_get_dropdownList, parameters);

        return await _context.Database
                .SqlQueryRaw<DropdownDto>(extendedSp, parameters)
                .AsNoTracking()
                .ToListAsync();
    }

    public async Task<IList<UserDropdownDto>> GetUserDropdownListAsync(DropdownRequest request)
    {
        var parameters = GetParameter(request);
        var extendedSp = SpNameHelper.GetSpName(Sp.usp_get_dropdownList, parameters);

        return await _context.Database
                .SqlQueryRaw<UserDropdownDto>(extendedSp, parameters)
                .AsNoTracking()
                .ToListAsync();
    }

    private static SqlParameter[] GetParameter(DropdownRequest request)
    {
        return
        [
            new SqlParameter("@Id", request.Id),
            new SqlParameter("@Param1", request.Param1 ?? (object)DBNull.Value),
            new SqlParameter("@Param2", request.Param2 ?? (object)DBNull.Value),
            new SqlParameter("@Param3", request.Param3 ?? (object)DBNull.Value),
            new SqlParameter("@Param4", request.Param4 ?? (object)DBNull.Value)
        ];
    }
}
