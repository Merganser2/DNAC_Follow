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
        
        if (user != null)
        {
            return user;
        }
        
        throw new Exception("Failed to Get User");
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userToUpdate = _entityFramework.Users?
                                              .Where(uji => uji.UserId == user.UserId)
                                              .SingleOrDefault();
        if (userToUpdate is not null)
        {
            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.Email = user.Email;
            userToUpdate.Gender = user.Gender;
            userToUpdate.Active = user.Active;

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Get User for update");
        }

        throw new Exception("Failed to Update User");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        User? userToAdd = new()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Gender = user.Gender,
            Active = user.Active
        };

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

        if (jobInfo != null)
        {
            return jobInfo;
        }

        throw new Exception("Failed to Get Job Info");
    }

    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfo)
    {
        UserJobInfo? userJobInfoForUpdate = _entityFramework.UserJobInfo?
                                                            .Where(uji => uji.UserId == userJobInfo.UserId)
                                                            .SingleOrDefault();
        if (userJobInfoForUpdate is not null)
        {
            userJobInfoForUpdate.Department = userJobInfo.Department;
            userJobInfoForUpdate.JobTitle = userJobInfo.JobTitle;

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Get UserJobInfo for update");
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

        if (salary != null)
        {
            return salary;
        }
        throw new Exception("Failed to Get User Salary");
    }

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalary)
    {
        UserSalary? userSalaryForUpdate = _entityFramework.UserSalary?
                                                          .Where(u => u.UserId == userSalary.UserId)
                                                          .SingleOrDefault();
        if (userSalaryForUpdate is not null)
        {
            userSalaryForUpdate.Salary = userSalary.Salary;

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Failed to Get User Salary for update");
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
