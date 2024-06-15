using HaefeleSoftware.Api.Domain.Entities;

namespace HaefeleSoftware.Api.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    
    Token GenerateRefreshToken(User user, string? ipAddress);
    
    int? ValidateToken(string? token);
}