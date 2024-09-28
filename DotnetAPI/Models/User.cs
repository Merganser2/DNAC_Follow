namespace DotnetAPI.Models;

// Best practice to name the POCO as singular instance (User, not Users)
public partial class User {
    public int UserId { get; set; }
	public string FirstName { get; set; } = "";
	public string LastName { get; set; } = "";
	public string Email { get; set; } = "";
	public string Gender { get; set; } = "";
	public Boolean Active { get; set; } 
}