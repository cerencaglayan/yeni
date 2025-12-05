using Microsoft.AspNetCore.Mvc;
using yeni.Configuration;
using yeni.Domain.Entities.Base;
using yeni.Domain.Requests;

namespace yeni.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (request.Name != "admin" || request.Password != "123456")
            return Unauthorized("Hatalı giriş");

        var user = new User
        {
            Id = 1,
            Name = request.Name
        };

        var accessToken = _tokenService.CreateAccessToken(user);
        var refreshToken = await _tokenService.CreateRefreshTokenAsync(user.Id);

        return Ok(new
        {
            accessToken,
            refreshToken
        });
    }

}
