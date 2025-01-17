﻿using System.Net;
using AutoMapper;
using FluentValidation;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Entities;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Authentication;

public sealed class AuthenticateEndpoint : IEndpoint
{
    private readonly IMapper _mapper;

    public AuthenticateEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("authenticate", async (AuthenticationRequest request, IMediator mediator, HttpContext context) =>
        {
            var command = _mapper.Map<AuthenticateCommand>(request);
            command.IpAddress = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            
            var result = await mediator.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1);
    }
}

public sealed class AuthenticateCommand : IRequest<Result<OnSuccess<AuthenticationResponse>, OnError>>
{
    public string Email { get; init; } = default!;

    public string Password { get; init; } = default!;
    
    public string? IpAddress { get; set; }
}

public sealed class AuthenticateCommandHandler : IRequestHandler<AuthenticateCommand,
    Result<OnSuccess<AuthenticationResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;
    private readonly ITokenRepository _tokenRepository;
    private readonly IJwtService _jwtService;

    public AuthenticateCommandHandler(ILogger logger, IUserRepository userRepository,
        IJwtService jwtService, ITokenRepository tokenRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _jwtService = jwtService;
        _tokenRepository = tokenRepository;
    }

    public async Task<Result<OnSuccess<AuthenticationResponse>, OnError>> Handle(AuthenticateCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.User? user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Account not found.");
            }

            if (!user.IsActive || user.IsDeleted)
            {
                return new OnError(HttpStatusCode.BadRequest, "Account is deactivated.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return new OnError(HttpStatusCode.BadRequest, "Invalid password.");
            }

            string token = _jwtService.GenerateToken(user);

            await ExpireUserTokens(user.Id, user.Email);
            Token refreshToken = _jwtService.GenerateRefreshToken(user, request.IpAddress);
            await _tokenRepository.AddTokenAsync(refreshToken);

            return new OnSuccess<AuthenticationResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new AuthenticationResponse
                {
                    Token = token,
                    RefreshToken = refreshToken.RefreshToken,
                    ExpiresAt = refreshToken.ExpiresAt
                }
            };
        }
        catch (Exception ex)
        {
            _logger.Error("Authentication request for {Email} failed: {Message}", request.Email, ex.Message);
            return new OnError(HttpStatusCode.BadRequest, "Authentication failed.");
        }
        finally
        {
            _logger.Information("Authentication request for {Email} has been processed.", request.Email);
        }
    }
    
    private async Task ExpireUserTokens(int userId, string? userEmail)
    {
        List<Token> refreshTokens = await _tokenRepository.GetUserTokensAsync(userId);

        foreach (var rtk in refreshTokens)
        {
            rtk.IsExpired = true;
            rtk.IsDeleted = true;
            rtk.LastModifiedBy = userEmail;
        }

        await _tokenRepository.UpdateTokensAsync(refreshTokens);
    } 
}

public sealed class AuthenticationValidator : AbstractValidator<AuthenticateCommand>
{
    public AuthenticationValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email is not valid.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}

public sealed class AuthenticationMappingProfile : Profile
{
    public AuthenticationMappingProfile()
    {
        CreateMap<AuthenticationRequest, AuthenticateCommand>();
    }
}

public sealed class AuthenticationRequest
{
    public string Email { get; init; } = default!;

    public string Password { get; init; } = default!;
}

public sealed class AuthenticationResponse
{
    public string Token { get; init; } = default!;

    public string RefreshToken { get; init; } = default!;

    public DateTime ExpiresAt { get; init; }
}