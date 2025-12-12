using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using yeni.Application.Services;
using yeni.Data;
using yeni.Domain.Error;
using yeni.Domain.Repositories;
using yeni.Domain.Requests;
using yeni.Infrastructure.Configuration;
using yeni.Infrastructure.Persistence;

namespace yeni.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(TokenService tokenService,UserService userService) : ControllerBase
{
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request,CancellationToken cancellationToken = default)
    {
        var result = await userService.Login(request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) :  Problem(result.Error.Message);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await userService.Logout(userId);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok();
    }
    
    
    
    
    
    
    
    
}
