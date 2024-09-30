using AutoMapper;
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
    private readonly IMapper _mapper;

    public UserEFController(IConfiguration config)
    {
        _entityFramework = new DataContextEF(config);
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserToAddDto, User>();
            cfg.CreateMap<UserSalary, UserSalary>().ReverseMap();
            cfg.CreateMap<UserJobInfo, UserJobInfo>().ReverseMap();
        }));
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User>? GetUsers()
    {
        IEnumerable<User>? users = _entityFramework.Users?.ToList();

        if (users is not null)
        {
            return users;
        }

        throw new Exception("No Users found in table");
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
    public IActionResult AddUser(UserToAddDto userDto)
    {
        User? userToAdd = _mapper.Map<User>(userDto);

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

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception($"Failed to Delete User {userId}");
        }

        throw new Exception($"Failed to Get User {userId} for Deletion");
    }

    /* 
        CRUD for UserJobInfo 
    */
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
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfoForUpdate)
    {
        UserJobInfo? userJobInfoToUpdate = _entityFramework.UserJobInfo?
                                                            .Where(uji => uji.UserId == userJobInfoForUpdate.UserId)
                                                            .SingleOrDefault();
        if (userJobInfoToUpdate is not null)
        {
            _mapper.Map(userJobInfoForUpdate, userJobInfoToUpdate);

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Updating UserJobInfo failed on Save");
        }
        throw new Exception("Failed to find UserJobInfo to Update");
    }

    [HttpPost("AddUserJobInfo")]
    public IActionResult AddUserJobInfo(UserJobInfo userJobInfoForInsert)
    {
        _entityFramework.UserJobInfo?.Add(userJobInfoForInsert);

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

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }            
            throw new Exception($"Failed to Delete UserJobInfo with userId {userId}");
        }
        throw new Exception($"Failed to Get UserJobInfo with userId {userId} for Deletion");
    }

    /* 
        CRUD for UserSalary 
    */
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
    public IActionResult EditUserSalary(UserSalary userSalaryForUpdate)
    {
        UserSalary? userSalaryToUpdate = _entityFramework.UserSalary?
                                                          .Where(u => u.UserId == userSalaryForUpdate.UserId)
                                                          .SingleOrDefault();
        if (userSalaryToUpdate is not null)
        {
            _mapper.Map(userSalaryForUpdate, userSalaryToUpdate);

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }

            throw new Exception("Updating User Salary failed on Save");
        }

        throw new Exception("Failed to get User Salary for Update");
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

            if (_entityFramework.SaveChanges() > 0)
            {
                return Ok();
            }
            throw new Exception($"Failed to Delete UserSalary with userId {userId}");
        }

        throw new Exception($"Failed to Get UserSalary with userId {userId} for Deletion");
    }
}
