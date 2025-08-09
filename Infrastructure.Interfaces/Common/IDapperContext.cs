using Microsoft.Data.SqlClient;

namespace Infrastructure.Interfaces.Common;

public interface IDapperContext
{
    SqlConnection CreateConnection();
}
