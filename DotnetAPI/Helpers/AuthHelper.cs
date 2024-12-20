using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers;

public class AuthHelper {

    private readonly IConfiguration _config;

    public AuthHelper(IConfiguration config)
    {
        _config = config;
    }

    public byte[] GetPasswordHash(string password, byte[] passwordSalt)
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

    public byte[] GetPasswordSalt()
    {
        byte[] passwordSalt = new byte[128 / 8]; //128 bytes

        using (RandomNumberGenerator rand = RandomNumberGenerator.Create())
        {
            rand.GetNonZeroBytes(passwordSalt);
        }

        return passwordSalt;
    }

    public string CreateToken(int userId)
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