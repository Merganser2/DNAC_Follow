namespace DotnetAPI.DTOs;

public class UserForLoginConfirmationDto {
    public byte[] PasswordHash {get; set;} = []; // new byte[0]
    public byte[] PasswordSalt {get; set;} = []; // or Array.Empty<byte>();
}

