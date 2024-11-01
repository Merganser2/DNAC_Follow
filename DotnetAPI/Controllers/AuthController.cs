using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    private readonly AuthHelper _authHelper;

    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _authHelper = new AuthHelper(config);
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistration)
    {
        // Check that entered passwords match
        if (userForRegistration.Password.Equals(userForRegistration.PasswordConfirm))
        {
            string sqlCheckUserExists = "SELECT Email FROM TutorialAppSchema.Auth WHERE Email = '" +
                                         userForRegistration.Email + "'";

            IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
            if (!existingUsers.Any())
            {
                byte[] passwordSalt = _authHelper.GetPasswordSalt();    
                byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                string sqlAddAuth = @"
                        INSERT INTO TutorialAppSchema.Auth ([Email],
                        [PasswordHash],
                        [PasswordSalt]) VALUES ('" + userForRegistration.Email +
                                                "', @PasswordHash, @PasswordSalt)";

                SqlParameter passwordSaltParameter = new("@PasswordSalt", SqlDbType.VarBinary)
                {
                    Value = passwordSalt
                };

                SqlParameter passwordHashParameter = new("@PasswordHash", SqlDbType.VarBinary)
                {
                    Value = passwordHash
                };

                List<SqlParameter> sqlParameters = [passwordSaltParameter, passwordHashParameter];

                if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters))
                {
                    // Add User to database (default to Active = 1 i.e. true, active)
                    string postSql = $@"INSERT INTO TutorialAppSchema.Users        
                                    ([FirstName],[LastName],[Email],[Gender],[Active]
                                    ) VALUES (
                                        '{userForRegistration.FirstName}',
                                        '{userForRegistration.LastName.Replace("'", "''")}',
                                        '{userForRegistration.Email}',
                                        '{userForRegistration.Gender}',
                                        '1' 
                                    )";
                    if (_dapper.ExecuteSql(postSql))
                    {
                        return Ok();
                    }
                    throw new Exception("Failed to Add User during Registration");
                }
            }
            throw new Exception("User with this email already exists!");
        }
        throw new Exception("Passwords do not match!");
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        string sqlForHashAndSalt = @"SELECT [PasswordHash],[PasswordSalt] 
            FROM TutorialAppSchema.Auth WHERE Email = '" + userForLogin.Email + "'";

        UserForLoginConfirmationDto userForConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

        byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

        // We must compare the values because Hashes are objects and thus will not be equal
        for (int index = 0; index < passwordHash.Length; index++)
        {
            if (passwordHash[index] != userForConfirmation.PasswordHash[index])
            {
                return StatusCode(401, "Incorrect password!");
            }
        }

        string sqlToGetUserId = @"SELECT [Users].[UserId]
                                  FROM DotNetCourseDatabase.TutorialAppSchema.Users
                                  WHERE Users.Email = '" + userForLogin.Email + "'";
        int userId = _dapper.LoadDataSingle<int>(sqlToGetUserId);

        return Ok(new Dictionary<string, string>{
                {"token", _authHelper.CreateToken(userId)}
            });
    }

    [HttpGet("RefreshToken")]
    public IActionResult RefreshToken()
    {
        // User from ControllerBase, not Model...
        // Get the first Claim found matching "userId" or null if not found
        // If we get a non-empty value, we know the token is valid, and can return them a new token
        string userId = User.FindFirst("userId")?.Value ?? "";

        if (userId.Length > 0) {

        // He's suggesting to use SQL call instead of converting to/from string/int
        string userIdSql = @"SELECT UserId FROM DotNetCourseDatabase.TutorialAppSchema.Users
                             WHERE UserId = " + userId;

        int userIdFromDb = _dapper.LoadDataSingle<int>(userIdSql);

        // But what if userId is "", thus
        return Ok(new Dictionary<string, string>{
            {"token", _authHelper.CreateToken(userIdFromDb)}
        });

        }
        throw new Exception("Token is invalid");
    }
}