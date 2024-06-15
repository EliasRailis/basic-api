using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HaefeleSoftware.Api.Infrastructure.Repositories;

public sealed class TokenRepository : ITokenRepository
{
    private readonly IDatabaseContext _context;

    public TokenRepository(IDatabaseContext context)
    {
        _context = context;
    }

    public async Task<List<Token>> GetUserTokensAsync(int userId)
    {
        return await _context.Tokens
            .Where(x => x.FK_UserId == userId && !x.IsExpired && !x.IsRevoked)
            .ToListAsync();
    }

    public async Task<bool> UpdateTokensAsync(IEnumerable<Token> tokens)
    {
        _context.Tokens.UpdateRange(tokens);
        return await _context.SaveChangesAsync(new CancellationToken()) > 0;
    }

    public async Task<bool> AddTokenAsync(Token token)
    {
        _context.Tokens.Add(token);
        return await _context.SaveChangesAsync(new CancellationToken()) > 0;
    }
}