using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
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
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;
    private readonly ApplicationDbContext _db;
    private readonly IUserRepository _userRepository;

    public AuthController(TokenService tokenService, IUserRepository userRepository)
    {
        _tokenService = tokenService;
        _userRepository = userRepository; 
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userRepository.GetByNameAsync(request.Name);

        if (user == null)
            return BadRequest(UserErrors.UserNotfound(request.Name));

        var isPasswordValid = PasswordHasher.Verify(
            request.Password,
            user.Password
        );

        if (!isPasswordValid)
            return BadRequest(UserErrors.InvalidCurrentPassword());

        var accessToken = _tokenService.CreateAccessToken(user);
        var refreshToken = await _tokenService.CreateRefreshTokenAsync(user.Id);

        return Ok(new
        {
            accessToken,
            refreshToken
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        await _tokenService.DeleteRefreshTokenAsync(userId);

        return Ok(new { message = "Logged out successfully" });
    }
}
