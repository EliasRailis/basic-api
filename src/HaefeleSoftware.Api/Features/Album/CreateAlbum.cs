using System.Net;
using AutoMapper;
using FluentValidation;
using HaefeleSoftware.Api.Application.Common.Utils;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Album;

public sealed class CreateAlbumEndpoint : IEndpoint
{
    private readonly IMapper _mapper;

    public CreateAlbumEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("albums/create", async (CreateAlbumRequest request, ISender sender) =>
        {
            var command = _mapper.Map<CreateAlbumCommand>(request);
            var result = await sender.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Admin);
    }
}

public sealed class CreateAlbumCommand : IRequest<Result<OnSuccess<CreateAlbumResponse>, OnError>>
{
    public int ArtistId { get; init; }
    
    public string Name { get; init; } = default!;

    public string YearOfRelease { get; init; } = default!;
    
    public int DurationInSeconds { get; init; }
}

public sealed class CreateAlbumCommandHandler : IRequestHandler<CreateAlbumCommand, 
    Result<OnSuccess<CreateAlbumResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly IAlbumRepository _albumRepository;
    private readonly CurrentUser? _currentUser;

    public CreateAlbumCommandHandler(ILogger logger, IAlbumRepository albumRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _albumRepository = albumRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<CreateAlbumResponse>, OnError>> Handle(CreateAlbumCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.Artist? artistAlbums = await _albumRepository
                .GetArtistAlbumsByIdAsync(request.ArtistId);
            
            if (artistAlbums is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Artist not found.");
            }

            if (artistAlbums.Albums.Any(x => !x.IsDeleted && x.Name == request.Name))
            {
                return new OnError(HttpStatusCode.BadRequest, "Album already exists.");
            }
            
            var album = new Domain.Entities.Album
            {
                FK_ArtistId = request.ArtistId,
                Name = request.Name.Trim(),
                YearOfRelease = request.YearOfRelease,
                Duration = TimeDuration.Format(request.DurationInSeconds),
                CreatedBy = _currentUser?.Email!
            };
            
            bool created = await _albumRepository.AddAlbumAsync(album);
            
            return new OnSuccess<CreateAlbumResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new CreateAlbumResponse
                {
                    IsSuccess = created,
                    AlbumId = album.Id,
                    Message = created ? "Album created." : "Album not created."
                }
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message);
            return new OnError(HttpStatusCode.BadRequest, ex.Message);
        }
        finally
        {
            _logger.Information("Request completed.");
        }
    }
}

public sealed class CreateAlbumValidator : AbstractValidator<CreateAlbumCommand>
{
    public CreateAlbumValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Album name is required.");

        RuleFor(x => x.YearOfRelease)
            .NotEmpty()
            .WithMessage("Year of release is required.");

        RuleFor(x => x.DurationInSeconds)
            .GreaterThan(0)
            .WithMessage("Duration in seconds must be greater than 0.")
            .NotEmpty()
            .WithMessage("Duration in seconds is required.");
    }
}

public sealed class CreateAlbumMappingProfile : Profile
{
    public CreateAlbumMappingProfile()
    {
        CreateMap<CreateAlbumRequest, CreateAlbumCommand>();
    }
}

public sealed class CreateAlbumRequest
{
    public int ArtistId { get; init; }
    
    public string Name { get; init; } = default!;

    public string YearOfRelease { get; init; } = default!;
    
    public int DurationInSeconds { get; init; }
}

public sealed class CreateAlbumResponse
{
    public bool IsSuccess { get; set; }
    
    public int AlbumId { get; set; }

    public string Message { get; set; } = default!;
}