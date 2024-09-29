namespace DotnetAPI.DTOs;

public class UserToAddDto {
	public string FirstName { get; set; } = "";
	public string LastName { get; set; } = "";
	public string Email { get; set; } = "";
	public string Gender { get; set; } = "";
	public Boolean Active { get; set; } 
}