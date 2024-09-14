
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace ComputerMapping.Data;

public class DataContextDapper
{
    private string? _connectionString;

    public DataContextDapper(IConfiguration configuration)
    {
        // GetConnectionString assumes "ConnectionStrings" section in settings.json
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public IEnumerable<T> GetRows<T>(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_connectionString);
        return dbConnection.Query<T>(sql);
    }

    public T GetSingleRow<T>(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_connectionString);
        return dbConnection.QuerySingle<T>(sql);
    }

    public bool ExecuteSql(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_connectionString);
        return (dbConnection.Execute(sql) > 0);
    }

    public int ExecuteSqlWithRowCount(string sql)
    {
        IDbConnection dbConnection = new SqlConnection(_connectionString);
        return dbConnection.Execute(sql);
    }
}