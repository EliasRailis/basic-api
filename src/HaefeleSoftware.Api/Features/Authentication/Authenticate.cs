using System.Net;
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
        app.MapPost("authenticate", async (AuthenticationRequest request, ISender sender, HttpContext context) =>
        {
            var command = _mapper.Map<AuthenticateCommand>(request);
            command.IpAddress = context.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            
            var result = await sender.Send(command);
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
    private readonly IUserRepository _repository;
    private readonly ITokenRepository _tokenRepository;
    private readonly IJwtService _jwtService;

    public AuthenticateCommandHandler(ILogger logger, IUserRepository repository,
        IJwtService jwtService, ITokenRepository tokenRepository)
    {
        _logger = logger;
        _repository = repository;
        _jwtService = jwtService;
        _tokenRepository = tokenRepository;
    }

    public async Task<Result<OnSuccess<AuthenticationResponse>, OnError>> Handle(AuthenticateCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            User? user = await _repository.GetUserByEmailAsync(request.Email);

            if (user is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Account not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return new OnError(HttpStatusCode.BadRequest, "Invalid password.");
            }

            string token = _jwtService.GenerateToken(user);
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