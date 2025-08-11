using System.Data;

namespace Infrastructure.Interfaces.Common;

public interface IDapperContext
{
    IDbConnection CreateConnection();
}
