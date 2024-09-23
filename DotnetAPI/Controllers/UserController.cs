using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

// Get some built-in functionality with [ApiController]
[ApiController]
[Route("[controller]")] // Gets name of class preceding "Controller" and assigns it to the Route of this controller
public class UserController : ControllerBase
{
    public UserController()
    {
        
    }

    [HttpGet("GetUsers/{testValue}", Name = "GetUser")]
    // public IActionResponse Test()     // IActionResponse is a result of an API call 
    // Can set testValue in browser w/ "user/test/?testValue=whatever" (notice no quotes around 'whatever').
    // Can make it instead "user/test/whatever" by adding parameter to HttpGet route above
    public string[] GetUsers(string testValue)
    {
        string[] responseArray = new string[] {"wh", "is", testValue};
        return responseArray;
    }
}
