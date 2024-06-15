using HaefeleSoftware.Api.Domain.Entities;

namespace HaefeleSoftware.Api.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
    
    Token GenerateRefreshToken(User user);
    
    int? ValidateToken(string? token);
}