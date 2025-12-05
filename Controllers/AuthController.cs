using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yeni.Configuration;
using yeni.Data;
using yeni.Domain.Entities.Base;
using yeni.Domain.Error;
using yeni.Domain.Repository;
using yeni.Domain.Requests;

namespace yeni.Controllers;

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

    /*
     * Eksikler:
     * - tokenları silmek için bir mekanizma yok.
     * 
     */
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

}
