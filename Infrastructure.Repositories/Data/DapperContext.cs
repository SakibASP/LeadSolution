using Infrastructure.Interfaces.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infrastructure.Repositories.Data;

public class DapperContext(IConfiguration configuration) : IDapperContext
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection");

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }
}

