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

public sealed class RevokeTokenEndpoint : IEndpoint
{
    private readonly IMapper _mapper;

    public RevokeTokenEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("revoke-token", async (RevokeTokenRequest request, ISender sender, HttpContext context) =>
        {
            var command = _mapper.Map<RevokeTokenCommand>(request);
            command.IpAddress = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();

            var result = await sender.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Admin);
    }
}

public sealed class RevokeTokenCommand : IRequest<Result<OnSuccess<RevokeTokenResponse>, OnError>>
{
    public string RefreshToken { get; init; } = default!;

    public string? IpAddress { get; set; }
}

public sealed class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand,
    Result<OnSuccess<RevokeTokenResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly ITokenRepository _tokenRepository;
    private readonly CurrentUser? _currentUser; 

    public RevokeTokenCommandHandler(ILogger logger, ITokenRepository tokenRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _tokenRepository = tokenRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<RevokeTokenResponse>, OnError>> Handle(RevokeTokenCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            Token? refreshToken = await _tokenRepository.GetRefreshTokenAsync(request.RefreshToken);

            if (refreshToken is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Refresh token not found");
            }

            refreshToken.IsRevoked = true;
            refreshToken.RevokedByIp = request.IpAddress;
            refreshToken.LastModifiedBy = _currentUser?.Email;
            bool updated = await _tokenRepository.UpdateTokenAsync(refreshToken);
            
            return new OnSuccess<RevokeTokenResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new RevokeTokenResponse
                {
                    IsRevoked = updated,
                    Message = updated ? "Refresh token revoked" : "Failed to revoke token"
                }
            };
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to revoke token {RefreshToken} with message {ErrroMessage}", 
                request.RefreshToken, ex.Message);
            return new OnError(HttpStatusCode.BadRequest, "Failed to revoke token");
        }
        finally
        {
            _logger.Information("Refresh token {RefreshToken} is revoked", request.RefreshToken);
        }
    }
}

public sealed class RevokeTokenValidator : AbstractValidator<RevokeTokenCommand>
{
    public RevokeTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required");
    }
}

public sealed class RevokeTokenMappingProfile : Profile
{
    public RevokeTokenMappingProfile()
    {
        CreateMap<RevokeTokenRequest, RevokeTokenCommand>();
    }
}

public sealed class RevokeTokenRequest
{
    public string RefreshToken { get; init; } = default!;
}

public sealed class RevokeTokenResponse
{
    public bool IsRevoked { get; init; }

    public required string Message { get; init; }
}