using Microsoft.Data.SqlClient;

namespace Common.Utils.Helper;

public static class SpNameHelper
{
    public static string GetSpName(string spName, SqlParameter[] parameters) => spName + " " + string.Join(" ,", parameters.Select(x => x.ParameterName));
    
}
