using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

// Get some built-in functionality with [ApiController]
[ApiController]
[Route("[controller]")] // Gets name of class preceding "Controller" and assigns it to the Route of this controller
public class UserController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestDbConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

   [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        // Note the 4000 character limit with Dapper; could remove 
        // spaces using + at end of each line if it were a big query
        string getUsersQuery = @"SELECT [UserId],
        [FirstName],[LastName],[Email],
        [Gender],[Active] 
        FROM TutorialAppSchema.Users";

        return _dapper.LoadData<User>(getUsersQuery);
    }

   [HttpGet("GetSingleUser")]
    public User GetUser(int userId)    
    {
        // Didn't have an issue without ToString but following his recommendation
        string sql = @$"
            SELECT * 
            FROM TutorialAppSchema.Users
            WHERE Users.UserId = {userId.ToString()}"; 

        return _dapper.LoadDataSingle<User>(sql);
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string postSql = $@"UPDATE TutorialAppSchema.Users 
        SET FirstName = '{user.FirstName}',
            LastName = '{user.LastName.Replace("'","''")}', 
            Email = '{user.Email}',
            Gender = '{user.Gender}',
            Active = '{user.Active}' 
        WHERE UserId = {user.UserId}";
 
        bool success = _dapper.ExecuteSql(postSql);
        return Ok(); // From ControllerBase 

        throw new Exception("Failed to Update User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(User user)
    {
        string postSql = $@"INSERT INTO TutorialAppSchema.Users        
        ([FirstName],[LastName],[Email],
         [Gender],[Active]
        ) VALUES (
            '{user.FirstName}',
            '{user.LastName.Replace("'","''")}',
            '{user.Email}',
            '{user.Gender}',
            '{user.Active}'
        )";

        System.Console.WriteLine("******************************");
        Console.WriteLine(postSql);
 
        bool success = _dapper.ExecuteSql(postSql);
        return Ok(); // From ControllerBase 

        throw new Exception("Failed to Add User");
    }
}
