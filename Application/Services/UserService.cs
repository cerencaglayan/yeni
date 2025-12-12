

using System.Security.Claims;
using yeni.Domain.Common;
using yeni.Domain.DTO.Responses;
using yeni.Domain.Entities.Base;
using yeni.Domain.Error;
using yeni.Domain.Repositories;
using yeni.Domain.Requests;
using yeni.Infrastructure.Configuration;

namespace yeni.Application.Services;

public class UserService(TokenService tokenService,IUserRepository userRepository)
{
    
    public async Task<Result<LoginResponse>> Login(LoginRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByNameAsync(request.Name, ct);

        if (user == null)
            return Result<LoginResponse>.Failure(UserErrors.UserNotfound(request.Name));

        var isPasswordValid = user.Password != null && PasswordHasher.Verify(
            request.Password,
            user.Password
        );

        if (!isPasswordValid)
            return Result<LoginResponse>.Failure(UserErrors.InvalidCurrentPassword());

        var accessToken = tokenService.CreateAccessToken(user);
        var refreshToken = await tokenService.CreateRefreshTokenAsync(user.Id);

        var response = new LoginResponse
        {
            accessToken = accessToken,
            refreshToken = refreshToken
        };

        return Result<LoginResponse>.Success(response);
    }

    public async Task<Result<bool>> Logout(int userId, CancellationToken ct = default)
    {
        await tokenService.DeleteRefreshTokenAsync(userId);
        
        return Result<bool>.Success(true);
    }

}