namespace DotnetAPI.DTOs;

// In addition to Password and confirm, we need the properties of a User
public class UserForRegistrationDto {
    public string Email {get; set;} = "";
    public string Password {get; set;} = "";
    public string PasswordConfirm {get; set;} = ""; 
    public string FirstName { get; set; } = "";
	public string LastName { get; set; } = "";
	public string Gender { get; set; } = "";
   
}

