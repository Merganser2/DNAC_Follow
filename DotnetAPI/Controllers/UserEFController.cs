using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

// Get some built-in functionality with [ApiController]
[ApiController]
[Route("[controller]")] // Gets name of class preceding "Controller" and assigns it to the Route of this controller
public class UserEFController : ControllerBase
{
    private readonly DataContextEF _entityFramework;

    public UserEFController(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User>? GetUsers()
    {
        IEnumerable<User>? users = _entityFramework.Users?.ToList();

        return users;
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User? GetUser(int userId)
    {
        User? user = _entityFramework.Users?.Where(user => user.UserId == userId)
                              .FirstOrDefault<User>();
        return user;
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userToUpdate = new User();
        userToUpdate.UserId = user.UserId;
        userToUpdate.FirstName = user.FirstName;
        userToUpdate.LastName = user.LastName;
        userToUpdate.Email = user.Email;
        userToUpdate.Gender = user.Gender;
        userToUpdate.Active = user.Active;        

        _entityFramework.Users?.Update(userToUpdate);

        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        } 

        throw new Exception("Failed to Update User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        User? userToAdd = new User();
        userToAdd.FirstName = user.FirstName;
        userToAdd.LastName = user.LastName;
        userToAdd.Email = user.Email;
        userToAdd.Gender = user.Gender;
        userToAdd.Active = user.Active;        

        _entityFramework.Users?.Add(userToAdd);
        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        } 

        throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userToRemove = _entityFramework.Users?.Where(u => u.UserId == userId).FirstOrDefault();

        if (userToRemove is not null)
        {
            _entityFramework.Users?.Remove(userToRemove);
        }

        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        } 

        throw new Exception($"Failed to Remove User {userId}");
    }

    /* 
        CRUD for UserJobInfo 
    */
    [HttpGet("GetAllUsersJobInfo")]
    public IEnumerable<UserJobInfo>? GetAllUsersJobInfo()
    {
        return _entityFramework.UserJobInfo?.ToList();
    }

    [HttpGet("UserJobInfo/{userId}")]
    public UserJobInfo? UserJobInfo(int userId)
    {
        UserJobInfo? jobInfo = _entityFramework.UserJobInfo?.Where(ji => ji.UserId == userId)
                                  .FirstOrDefault<UserJobInfo>();

        return jobInfo;
    }

    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    {
        UserJobInfo? userJobInfoForUpdate = new()
        {
            UserId = userJobInfo.UserId,
            Department = userJobInfo.Department,
            JobTitle = userJobInfo.JobTitle
        };

        _entityFramework.UserJobInfo?.Update(userJobInfoForUpdate);

        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        } 

        throw new Exception("Failed to Update UserJobInfo");
    }

    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfoForInsert)
    {
        _entityFramework.UserJobInfo?.Add(userJobInfoForInsert);
        _entityFramework.SaveChanges(); 

        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        } 

        throw new Exception("Failed to Add UserJobInfo");
    }

    [HttpDelete("DeleteUserJobInfo")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        UserJobInfo? jobInfoToRemove = _entityFramework.UserJobInfo?.Where(u => u.UserId == userId).FirstOrDefault();

        if (jobInfoToRemove is not null)
        {
            _entityFramework.UserJobInfo?.Remove(jobInfoToRemove);
        }

        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        } 
        throw new Exception($"Failed to Remove UserJobInfo {userId}");
    }

    /* 
        CRUD for UserSalary 
    */
    [HttpGet("GetAllUsersSalary")]
    public IEnumerable<UserSalary>? GetAllUsersSalary()
    {
        return _entityFramework.UserSalary?.ToList();
    }

    [HttpGet("UserSalary/{userId}")]
    public UserSalary? UserSalary(int userId)
    {
        UserSalary? salary = _entityFramework.UserSalary?.Where(s => s.UserId == userId)
                                .FirstOrDefault<UserSalary>();

        return salary;
    }

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        UserSalary? userSalaryForUpdate = new()
        {
            UserId = userSalary.UserId,
            Salary = userSalary.Salary
        };

        _entityFramework.UserSalary?.Update(userSalaryForUpdate);

        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        } 

        throw new Exception("Failed to update User Salary");
    }

    [HttpPost("AddUserSalary")]
    public IActionResult AddUserSalary(UserSalary userSalaryForInsert)
    {
        _entityFramework.UserSalary?.Add(userSalaryForInsert);

        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        } 

        throw new Exception("Failed to Add UserSalary");
    }

    [HttpDelete("DeleteUserSalary")]
    public IActionResult DeleteUserSalary(int userId)
    {
        UserSalary? salaryToRemove = _entityFramework.UserSalary?
                                        .Where(u => u.UserId == userId)
                                        .FirstOrDefault();

        if (salaryToRemove is not null)
        {
            // Was getting a warning on UserSalary, even though it can't be null if we get here
            _entityFramework.UserSalary?.Remove(salaryToRemove);
        }

        if (_entityFramework.SaveChanges() > 0)
        {
            return Ok();
        } 
        throw new Exception($"Failed to Remove UserSalary {userId}");
    }
}
