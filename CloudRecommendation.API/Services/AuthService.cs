using CloudRecommendation.Api.Data;
using CloudRecommendationApi.DTOs;
using CloudRecommendationApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CloudRecommendationApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthService(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existing = await _db.Users.AnyAsync(u => u.Email == dto.Email);
        if (existing)
            throw new AuthException("An account with this email already exists.");

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email.ToLowerInvariant(),
            Company = dto.Company,
            Role = "User"
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _db.Users.Add(user);
        _db.AuditLogs.Add(new AuditLog { Action = "UserRegistered", Details = $"Email={user.Email}" });
        await _db.SaveChangesAsync();

        var (token, expiresAt) = _tokenService.GenerateToken(user);
        return new AuthResponseDto { Token = token, ExpiresAt = expiresAt, Name = user.Name, Email = user.Email };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email.ToLowerInvariant());
        if (user is null)
            throw new AuthException("Invalid email or password.");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new AuthException("Invalid email or password.");

        _db.AuditLogs.Add(new AuditLog { UserId = user.Id, Action = "UserLoggedIn" });
        await _db.SaveChangesAsync();

        var (token, expiresAt) = _tokenService.GenerateToken(user);
        return new AuthResponseDto { Token = token, ExpiresAt = expiresAt, Name = user.Name, Email = user.Email };
    }

    public async Task<UserProfileDto?> GetProfileAsync(int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is null) return null;

        return new UserProfileDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Company = user.Company,
            CreatedAt = user.CreatedAt
        };
    }
}
