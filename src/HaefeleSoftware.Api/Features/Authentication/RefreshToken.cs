using System.Net;
using AutoMapper;
using FluentValidation;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Entities;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Authentication;

public sealed class RefreshTokenEndpoint : IEndpoint
{
    private readonly IMapper _mapper;

    public RefreshTokenEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("refresh-token", async (RefreshTokenRequest request, ISender sender, HttpContext context) =>
        {
            var command = _mapper.Map<RefreshTokenCommand>(request);
            command.IpAddress = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();

            var result = await sender.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1);
    }
}

public sealed class RefreshTokenCommand : IRequest<Result<OnSuccess<RefreshTokenResponse>, OnError>>
{
    public string RefreshToken { get; init; } = default!;

    public string? IpAddress { get; set; }
}

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand,
    Result<OnSuccess<RefreshTokenResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly ITokenRepository _tokenRepository;
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    private readonly CurrentUser? _currentUser;

    public RefreshTokenCommandHandler(ILogger logger, ITokenRepository tokenRepository,
        IJwtService jwtService, ICurrentUserService currentUser, IUserRepository userRepository)
    {
        _logger = logger;
        _tokenRepository = tokenRepository;
        _jwtService = jwtService;
        _userRepository = userRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<RefreshTokenResponse>, OnError>> Handle(RefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            Token? oldRefreshToken = await _tokenRepository.GetRefreshTokenAsync(request.RefreshToken);

            if (oldRefreshToken is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Refresh token not found.");
            }

            if (oldRefreshToken.ExpiresAt < DateTime.UtcNow || oldRefreshToken.IsRevoked)
            {
                await ExpireUserTokens(_currentUser!.Id, _currentUser.Email);
                return new OnError(HttpStatusCode.Unauthorized, "Invalid refresh token.");
            }

            string jwtToken = _jwtService.GenerateToken(oldRefreshToken.User);
            Token newRefreshToken = _jwtService.GenerateRefreshToken(oldRefreshToken.User, request.IpAddress);
            newRefreshToken.ExpiresAt = oldRefreshToken.ExpiresAt;
            await _tokenRepository.AddTokenAsync(newRefreshToken);

            return new OnSuccess<RefreshTokenResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new RefreshTokenResponse
                {
                    Token = jwtToken,
                    RefreshToken = newRefreshToken.RefreshToken,
                    ExpiresAt = newRefreshToken.ExpiresAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.Error("Error refreshing token {RefreshToken} failed {ErrorMessage}.",
                request.RefreshToken, ex.Message);
            return new OnError(HttpStatusCode.BadRequest, "Error refreshing token.");
        }
        finally
        {
            _logger.Information("Refresh token {RefreshToken} expired, new token issued.", request.RefreshToken);
        }
    }

    private async Task ExpireUserTokens(int userId, string? userEmail)
    {
        List<Token> refreshTokens = await _tokenRepository.GetUserTokensAsync(userId);

        foreach (var rtk in refreshTokens)
        {
            rtk.IsExpired = true;
            rtk.LastModifiedBy = userEmail;
        }

        await _tokenRepository.UpdateTokensAsync(refreshTokens);
    }
}

public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Token is required.");
    }
}

public sealed class RefreshTokenMappingProfile : Profile
{
    public RefreshTokenMappingProfile()
    {
        CreateMap<RefreshTokenRequest, RefreshTokenCommand>();
    }
}

public sealed class RefreshTokenRequest
{
    public string RefreshToken { get; init; } = default!;
}

public sealed class RefreshTokenResponse
{
    public string Token { get; init; } = default!;

    public string RefreshToken { get; init; } = default!;

    public DateTime ExpiresAt { get; init; }
}