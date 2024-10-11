using System.Data;
using System.Security.Cryptography;
using System.Text;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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
            if (existingUsers.Count() == 0)
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
            // TODO: Remove
            Console.Write($" {index}: {passwordHash[index]}");
            if (passwordHash[index] != userForConfirmation.PasswordHash[index]){
                return StatusCode(401, "Incorrect password!");
            }            
        }
        Console.WriteLine();
        return Ok();
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
}