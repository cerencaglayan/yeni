using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using yeni.Domain.Entities.Base;
using yeni.Domain.Error;
using Error = yeni.Domain.Error.Error;

namespace yeni.Controllers;

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