using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using yeni.Data;
using yeni.Domain.Entities.Base;
using yeni.Infrastructure.Persistence;

namespace yeni.Application.Services;

public class TokenService
{
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _db;
    
    public TokenService(IConfiguration config, ApplicationDbContext db)
    {
        _config = config;
        _db = db;

    }

    public string CreateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(_config["Jwt:ExpireMinutes"]!)
            ),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> CreateRefreshTokenAsync(int userId)
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var entity = RefreshToken.Create(
            refreshToken,
            DateTime.UtcNow.AddDays(
                int.Parse(_config["Jwt:RefreshTokenExpireDays"]!)
            ), userId
        );

        _db.RefreshTokens.Add(entity);
        await _db.SaveChangesAsync();

        return refreshToken;
    }


    public async Task<int> DeleteRefreshTokenAsync(int userId)
    {
        var refreshTokens = await _db.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked)
            .ToListAsync();

        _db.RefreshTokens.RemoveRange(refreshTokens);

        await _db.SaveChangesAsync();
        return userId;
    }
}