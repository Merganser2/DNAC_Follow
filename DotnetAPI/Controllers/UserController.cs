using DotnetAPI.Data;
using DotnetAPI.DTOs;
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

   [HttpGet("GetSingleUser/{userId}")]
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
        string updateSql = $@"UPDATE TutorialAppSchema.Users 
        SET FirstName = '{user.FirstName}',
            LastName = '{user.LastName.Replace("'","''")}', 
            Email = '{user.Email}',
            Gender = '{user.Gender}',
            Active = '{user.Active}' 
        WHERE UserId = {user.UserId}";

        if (_dapper.ExecuteSql(updateSql))
        {
            return Ok(); // From ControllerBase 
        }

        throw new Exception("Failed to Update User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string postSql = $@"INSERT INTO TutorialAppSchema.Users        
        ([FirstName],[LastName],[Email],[Gender],[Active]
        ) VALUES (
            '{user.FirstName}',
            '{user.LastName.Replace("'","''")}',
            '{user.Email}',
            '{user.Gender}',
            '{user.Active}'
        )";
 
        if (_dapper.ExecuteSql(postSql))
        {
            return Ok(); // From ControllerBase 
        }

        throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId){

        string deleteSql = $@"DELETE FROM TutorialAppSchema.Users
                              WHERE UserId = {userId}";

        _dapper.ExecuteSql(deleteSql);

        return Ok();
        throw new Exception($"Failed to Remove User {userId}");
    }

    /* 
        CRUD for UserJobInfo 
    */
    [HttpGet("GetAllUsersJobInfo")]
    public IEnumerable<UserJobInfo> GetAllUsersJobInfo()
    {
        string getUserJobInfoQuery = @"SELECT [UserId],
        [Department],[JobTitle]
        FROM TutorialAppSchema.UserJobInfo";

        return _dapper.LoadData<UserJobInfo>(getUserJobInfoQuery);
    }

   [HttpGet("UserJobInfo/{userId}")]
    public UserJobInfo UserJobInfo(int userId)    
    {
        string sql = @$"
            SELECT * 
            FROM TutorialAppSchema.UserJobInfo
            WHERE UserJobInfo.UserId = {userId}"; 

        return _dapper.LoadDataSingle<UserJobInfo>(sql);
    }

    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfoForInsert)
    {
        string postSql = $@"UPDATE TutorialAppSchema.UserJobInfo 
        SET Department = '{userJobInfoForInsert.Department}',
            JobTitle = '{userJobInfoForInsert.JobTitle}'
        WHERE UserId = {userJobInfoForInsert.UserId}";
 
        if (_dapper.ExecuteSql(postSql)) {
            return Ok(userJobInfoForInsert); 
        }

        throw new Exception("Failed to Update UserJobInfo");
    }

    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfoForInsert)
    {
        string postSql = $@"INSERT INTO TutorialAppSchema.UserJobInfo        
        ([UserId],[Department],[JobTitle]
        ) VALUES (
            '{userJobInfoForInsert.UserId}',
            '{userJobInfoForInsert.Department}',
            '{userJobInfoForInsert.JobTitle}'
        )";

        if (_dapper.ExecuteSqlWithRowCount(postSql) > 0)
        {
            return Ok(userJobInfoForInsert);
        }

        throw new Exception("Failed to Add UserJobInfo");
    }

    [HttpDelete("DeleteUserJobInfo")]
    public IActionResult DeleteUserJobInfo(int userId){

        string deleteSql = $@"DELETE FROM TutorialAppSchema.UserJobInfo
                              WHERE UserId = {userId}";

        if (_dapper.ExecuteSql(deleteSql)) {
            return Ok(); 
        }

        return Ok();
        throw new Exception($"Failed to Remove UserJobInfo {userId}");
    }

    /* 
        CRUD for UserSalary 
    */
    [HttpGet("GetAllUsersSalary")]
    public IEnumerable<UserSalary> GetAllUsersSalary()
    {
        string getUserSalaryQuery = @"SELECT [UserId],[Salary]
        FROM TutorialAppSchema.UserSalary";

        return _dapper.LoadData<UserSalary>(getUserSalaryQuery);
    }

   [HttpGet("UserSalary/{userId}")]
    public UserSalary UserSalary(int userId)    
    {
        string sql = @$"
            SELECT [UserId],[Salary] 
            FROM TutorialAppSchema.UserSalary
            WHERE UserSalary.UserId = {userId}"; 

        return _dapper.LoadDataSingle<UserSalary>(sql);
    }

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalaryForInsert)
    {
        string postSql = $@"UPDATE TutorialAppSchema.UserSalary 
        SET Salary = '{userSalaryForInsert.Salary}'
        WHERE UserId = {userSalaryForInsert.UserId}";
 
        if (_dapper.ExecuteSql(postSql)) {
            return Ok(userSalaryForInsert); 
        }

        throw new Exception("Failed to Update UserSalary");
    }

    [HttpPost("AddUserSalary")]
    public IActionResult AddUserSalary(UserSalary userSalaryForInsert)
    {
        string postSql = $@"INSERT INTO TutorialAppSchema.UserSalary        
        ([UserId],[Salary]
        ) VALUES (
            '{userSalaryForInsert.UserId}',
            '{userSalaryForInsert.Salary}'
        )";

        if (_dapper.ExecuteSqlWithRowCount(postSql) > 0)
        {
            return Ok(userSalaryForInsert);
        }

        throw new Exception("Failed to Add UserSalary");
    }

    [HttpDelete("DeleteUserSalary")]
    public IActionResult DeleteUserSalary(int userId){

        string deleteSql = $@"DELETE FROM TutorialAppSchema.UserSalary
                              WHERE UserId = {userId}";

        if (_dapper.ExecuteSql(deleteSql)) {
            return Ok(); 
        }

        return Ok();
        throw new Exception($"Failed to Remove UserSalary {userId}");
    }
}
