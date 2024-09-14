using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ComputerInfo.Data;

public class DataContextDapper
{
    // private IConfiguration _config;

    // Below for Apple/Linux, if we were actually hard-coding it
    // private string _connectionString = "Server=localhost;Database=DotNetCourseDatabase;Trusted_connection=false;TrustServerCertificate=True;User Id=sa;Password=fakePasswordSQLConnect;";
    private string? _connectionString;

    public DataContextDapper(IConfiguration configuration)
    {
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