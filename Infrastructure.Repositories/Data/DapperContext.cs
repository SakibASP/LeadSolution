using Infrastructure.Interfaces.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Infrastructure.Repositories.Data;


/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>


public sealed class DapperContext(IConfiguration configuration) : IDapperContext
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;
    public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
}

