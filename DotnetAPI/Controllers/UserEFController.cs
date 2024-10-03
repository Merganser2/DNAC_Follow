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
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserEFController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserToAddDto, User>();
            cfg.CreateMap<User, User>().ReverseMap();
            cfg.CreateMap<UserSalary, UserSalary>().ReverseMap();
            cfg.CreateMap<UserJobInfo, UserJobInfo>().ReverseMap();
        }));
    }

    [HttpGet("GetUsers")]
    public IEnumerable<User>? GetUsers()
    {
        return _userRepository.GetUsers();
    }

    [HttpGet("GetSingleUser/{userId}")]
    public User? GetUser(int userId)
    {
        return _userRepository.GetSingleUser(userId);
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User userForUpdate)
    {
        User? userToUpdate = _userRepository.GetSingleUser(userForUpdate.UserId);
        if (userToUpdate is not null)
        {
            _mapper.Map(userForUpdate, userToUpdate);

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception("Failed to Update User on save");

        }
        throw new Exception("Failed to Get User for update");
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto userDto)
    {
        User? userToAdd = _mapper.Map<User>(userDto);

        _userRepository.AddEntity<User>(userToAdd);
        
        if (_userRepository.SaveChanges())
        {
            return Ok();
            throw new Exception("Adding user failed on save");
        }

        throw new Exception("Failed to Add User");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {

        User? userToRemove = _userRepository.GetSingleUser(userId);

        if (userToRemove is not null)
        {
           _userRepository.RemoveEntity(userToRemove);

            if (_userRepository.SaveChanges())
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
        UserJobInfo? jobInfo = _userRepository.GetUserJobInfo(userId);

        if (jobInfo != null)
        {
            return jobInfo;
        }

        throw new Exception("Failed to Get Job Info");
    }

    [HttpPut("EditUserJobInfo")]
    public IActionResult EditUserJobInfo(UserJobInfo userJobInfoForUpdate)
    {
        UserJobInfo? userJobInfoToUpdate = _userRepository.GetUserJobInfo(userJobInfoForUpdate.UserId);

        if (userJobInfoToUpdate is not null)
        {
            _mapper.Map(userJobInfoForUpdate, userJobInfoToUpdate);

            if (_userRepository.SaveChanges())
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
        _userRepository.AddEntity(userJobInfoForInsert);

        if (_userRepository.SaveChanges())
        {
            return Ok();
        }

        throw new Exception("Failed to Add UserJobInfo");
    }

    [HttpDelete("DeleteUserJobInfo")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        UserJobInfo? jobInfoToRemove = _userRepository.GetUserJobInfo(userId);

        if (jobInfoToRemove is not null)
        {
           _userRepository.RemoveEntity(jobInfoToRemove);
            if (_userRepository.SaveChanges())
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
        UserSalary? salary =  _userRepository.GetUserSalary(userId);

        if (salary != null)
        {
            return salary;
        }
        throw new Exception("Failed to Get User Salary");
    }

    [HttpPut("EditUserSalary")]
    public IActionResult EditUserSalary(UserSalary userSalaryForUpdate)
    {
        UserSalary? userSalaryToUpdate = _userRepository.GetUserSalary(userSalaryForUpdate.UserId);

        if (userSalaryToUpdate is not null)
        {
            _mapper.Map(userSalaryForUpdate, userSalaryToUpdate);

            if (_userRepository.SaveChanges())
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
        _userRepository.AddEntity(userSalaryForInsert);

        if (_userRepository.SaveChanges())
        {
            return Ok();
        }

        throw new Exception("Failed to Add UserSalary");
    }

    [HttpDelete("DeleteUserSalary")]
    public IActionResult DeleteUserSalary(int userId)
    {
        UserSalary? salaryToRemove = _userRepository.GetUserSalary(userId);

        if (salaryToRemove is not null)
        {
            // Was getting a warning on UserSalary, even though it can't be null if we get here
            _userRepository.RemoveEntity(salaryToRemove);

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new Exception($"Failed to Delete UserSalary with userId {userId}");
        }

        throw new Exception($"Failed to Get UserSalary with userId {userId} for Deletion");
    }
}
