using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using yeni.Domain.Entities.Base;


namespace yeni.API.Controllers;

public class UserController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult Get()
    {
        return Ok(new User()
        {
            Id = 1
        });
            
    }
}