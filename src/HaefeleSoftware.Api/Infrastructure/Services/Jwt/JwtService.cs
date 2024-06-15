using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Entities;
using HaefeleSoftware.Api.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HaefeleSoftware.Api.Infrastructure.Services.Jwt;

public sealed class JwtService : IJwtService
{
    private readonly JwtSettings _settings;
    private readonly IDateTimeService _dateTimeService;
    private readonly IDatabaseContext _context;

    public JwtService(IOptions<JwtSettings> settings, IDateTimeService dateTimeService, IDatabaseContext context)
    {
        _settings = settings.Value;
        _dateTimeService = dateTimeService;
        _context = context;
    }

    public string GenerateToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.UTF8.GetBytes(_settings.AccessTokenSecret);

        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(IdentityRoles.ClaimName, GetRoleName((Roles)user.FK_RoleId)),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Email),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("id", user.Id.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _dateTimeService.Now.AddMinutes(10),
            SigningCredentials = signingCredentials,
            Issuer = _settings.Issuer,
            Audience = _settings.Audience
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public Token GenerateRefreshToken(User user)
    {
        throw new NotImplementedException();
    }

    public Token GenerateRefreshToken(User user, string ipAddress)
    {
        return new Token
        {
            Created = _dateTimeService.Now,
            CreatedBy = user.Email,
            RefreshToken = GetUniqueRefreshToken(),
            ExpiresAt = _dateTimeService.Now.AddDays(10),
            CreatedByIp = ipAddress,
            FK_UserId = user.Id,
            IsDeleted = false,
            IsExpired = false,
            IsRevoked = false
        };
    }

    private string GetUniqueRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)).Replace("/", "");
        bool isTokenUnique = !_context.Tokens.Any(x => x.RefreshToken == token);
        return !isTokenUnique ? GetUniqueRefreshToken() : token;
    }

    public int? ValidateToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.UTF8.GetBytes(_settings.AccessTokenSecret);

        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience,
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken securityToken);

        var jwtToken = (JwtSecurityToken) securityToken;
        var id = jwtToken.Claims.First(x => x.Type == "id").Value;
        return int.Parse(id);
    }

    private static string GetRoleName(Roles role) => role switch
    {
        Roles.Admin => IdentityRoles.Admin,
        Roles.Customer => IdentityRoles.Customer,
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
    };
}