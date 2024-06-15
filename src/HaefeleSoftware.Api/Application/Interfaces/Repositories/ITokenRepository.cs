using HaefeleSoftware.Api.Domain.Entities;

namespace HaefeleSoftware.Api.Application.Interfaces.Repositories;

public interface ITokenRepository
{
    Task<List<Token>> GetUserTokensAsync(int userId);
    
    Task<bool> UpdateTokensAsync(IEnumerable<Token> tokens);
    
    Task<bool> UpdateTokenAsync(Token token);
    
    Task<bool> AddTokenAsync(Token token);
    
    Task<Token?> GetRefreshTokenAsync(string token);
}