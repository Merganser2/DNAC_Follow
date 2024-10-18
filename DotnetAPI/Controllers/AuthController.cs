using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers;

public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
        _dapper = new DataContextDapper(config);
    }

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
                byte[] passwordSalt = GetPasswordSalt();
                byte[] passwordHash = GetPasswordHash(userForRegistration.Password, passwordSalt);

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

    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLogin)
    {
        string sqlForHashAndSalt = @"SELECT [PasswordHash],[PasswordSalt] 
            FROM TutorialAppSchema.Auth WHERE Email = '" + userForLogin.Email + "'";

        UserForLoginConfirmationDto userForConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

        byte[] passwordHash = GetPasswordHash(userForLogin.Password, userForConfirmation.PasswordSalt);

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

        Console.WriteLine();
        return Ok(new Dictionary<string, string>{
                {"token", CreateToken(userId)}
            });
    }

    private byte[] GetPasswordSalt()
    {
        byte[] passwordSalt = new byte[128 / 8]; //128 bytes

        using (RandomNumberGenerator rand = RandomNumberGenerator.Create())
        {
            rand.GetNonZeroBytes(passwordSalt);
        }

        return passwordSalt;
    }

    private byte[] GetPasswordHash(string password, byte[] passwordSalt)
    {
        string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value +
            Convert.ToBase64String(passwordSalt);

        return KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 1000000,
            numBytesRequested: 256 / 8
        );
    }

    private string CreateToken(int userId)
    {
        Claim[] claims = [
            new Claim("userId", userId.ToString())
        ];

        string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

        // Coalesce to null to solve the warning. Also NOTE, the string must be greater than 512 bits, or will get
        // ArgumentOutOfRangeException: IDX10720: Unable to create KeyedHashAlgorithm for algorithm 'http://www.w3.org/2001/04/xmldsig-more#hmac-sha512',  
        // the key size must be greater than: '512' bits, key has '384' bits. See https://aka.ms/IdentityModel/UnsafeRelaxHmacKeySizeValidation (Parameter 'keyBytes')
        // When running tokenHandler.CreateToken below
        SymmetricSecurityKey symmetricTokenKey = new(Encoding.UTF8.GetBytes(tokenKeyString ?? ""));

        SigningCredentials credentials = new(symmetricTokenKey, SecurityAlgorithms.HmacSha512Signature);

        SecurityTokenDescriptor descriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            SigningCredentials = credentials,
            Expires = DateTime.Now.AddDays(1)
        };

        // Just a class that has some of the methods we need to 
        JwtSecurityTokenHandler tokenHandler = new();

        SecurityToken token = tokenHandler.CreateToken(descriptor);

        return tokenHandler.WriteToken(token); // converts it to a string that...
    }
}