using System.Net;
using AutoMapper;
using FluentValidation;
using HaefeleSoftware.Api.Application.Common.Utils;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Types;
using HaefeleSoftware.Api.Features.Artist;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Album;

public sealed class UpdateAlbumEndpoint : IEndpoint
{
    private readonly IMapper _mapper;

    public UpdateAlbumEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("albums/update", async (UpdateAlbumRequest request, ISender sender) =>
        {
            var command = _mapper.Map<UpdateAlbumCommand>(request);
            var result = await sender.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Admin);
    }
}

public sealed class UpdateAlbumCommand : IRequest<Result<OnSuccess<UpdateAlbumResponse>, OnError>>
{
    public int AlbumId { get; init; }
    
    public string Name { get; init; } = default!;

    public string YearOfRelease { get; init; } = default!;
    
    public int DurationIsSeconds { get; init; }
}

public sealed class UpdateAlbumCommandHandler : IRequestHandler<UpdateAlbumCommand, 
    Result<OnSuccess<UpdateAlbumResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly IAlbumRepository _albumRepository;
    private readonly CurrentUser? _currentUser;

    public UpdateAlbumCommandHandler(ILogger logger, IAlbumRepository albumRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _albumRepository = albumRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<UpdateAlbumResponse>, OnError>> Handle(UpdateAlbumCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.Album? album = await _albumRepository.GetAlbumByIdAsync(request.AlbumId);
            
            if (album is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Album not found.");
            }
            
            Domain.Entities.Artist? artistAlbums = await _albumRepository
                .GetArtistAlbumsByIdAsync(album.FK_ArtistId);

            if (artistAlbums?.Albums.Where(x => !x.IsDeleted).Any(x => x.Name == request.Name) is true)
            {
                return new OnError(HttpStatusCode.BadRequest, "Album name already exists.");
            }
            
            album.Name = request.Name.Trim();
            album.YearOfRelease = request.YearOfRelease;
            album.Duration = TimeDuration.Format(request.DurationIsSeconds);
            album.LastModifiedBy = _currentUser?.Email;
            bool updated = await _albumRepository.UpdateAlbumAsync(album);
            
            return new OnSuccess<UpdateAlbumResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new UpdateAlbumResponse
                {
                    IsUpdated = updated,
                    Message = updated ? "Album updated." : "Album not updated."
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

public sealed class UpdateAlbumValidator : AbstractValidator<UpdateAlbumCommand>
{
    public UpdateAlbumValidator()
    {
        RuleFor(x => x.AlbumId)
            .NotEmpty()
            .WithMessage("Album ID is required.");
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.YearOfRelease)
            .NotEmpty()
            .WithMessage("Year of release is required.");
        
        RuleFor(x => x.DurationIsSeconds)
            .NotEmpty()
            .WithMessage("Duration in seconds is required.")
            .GreaterThan(0)
            .WithMessage("Duration in seconds must be greater than 0.");
    }
}

public sealed class UpdateAlbumMappingProfile : Profile
{
    public UpdateAlbumMappingProfile()
    {
        CreateMap<UpdateArtistRequest, UpdateArtistCommand>();
    }
}

public sealed class UpdateAlbumRequest
{
    public int AlbumId { get; set; }
    
    public string Name { get; init; } = default!;

    public string YearOfRelease { get; init; } = default!;
    
    public int DurationIsSeconds { get; init; }
}

public sealed class UpdateAlbumResponse
{
    public bool IsUpdated { get; init; }
    
    public string Message { get; init; } = default!;
}