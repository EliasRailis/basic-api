using HaefeleSoftware.Api.Domain.Entities;

namespace HaefeleSoftware.Api.Application.Interfaces.Repositories;

public interface ITokenRepository
{
    Task<List<Token>> GetUserTokensAsync(int userId);
    
    Task<bool> UpdateTokensAsync(IEnumerable<Token> tokens);
    
    Task<bool> AddTokenAsync(Token token);
}