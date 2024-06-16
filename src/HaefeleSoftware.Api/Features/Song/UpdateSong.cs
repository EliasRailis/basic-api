using System.Net;
using AutoMapper;
using FluentValidation;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Song;

public sealed class UpdateSongEndpoint : IEndpoint
{
    private readonly IMapper _mapper;

    public UpdateSongEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("songs/update", async (UpdateSongRequest request, ISender sender) =>
        {
            var command = _mapper.Map<UpdateSongCommand>(request);
            var result = await sender.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Admin);
    }
}

public sealed class UpdateSongCommand : IRequest<Result<OnSuccess<UpdateSongResponse>, OnError>>
{
    public int SongId { get; init; }
    
    public string Name { get; init; } = default!;
    
    public int Duration { get; init; }
}

public sealed class UpdateSongCommandHandler : IRequestHandler<UpdateSongCommand,
    Result<OnSuccess<UpdateSongResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly ISongRepository _songRepository;
    private readonly CurrentUser? _currentUser;

    public UpdateSongCommandHandler(ILogger logger, ISongRepository songRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _songRepository = songRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<UpdateSongResponse>, OnError>> Handle(UpdateSongCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.Song? song = await _songRepository.GetSongByIdAsync(request.SongId);
            
            if (song is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Song not found.");
            }
            
            Domain.Entities.Album? albumSongs = await _songRepository.GetAlbumSongsAsync(song.FK_AlbumId);
            
            if (albumSongs is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Album not found.");
            }

            if (albumSongs.Songs.Where(x => !x.IsDeleted).Any(x => x.Name == request.Name.Trim()))
            {
                return new OnError(HttpStatusCode.BadRequest, "Song name already exists.");
            }
            
            song.Name = request.Name.Trim();
            song.Duration = SecondsToTime(request.Duration);
            song.LastModifiedBy = _currentUser?.Email;
            bool updated = await _songRepository.UpdateSongAsync(song);
            
            return new OnSuccess<UpdateSongResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new UpdateSongResponse
                {
                    IsUpdated = updated,
                    Message = updated ? "Song updated." : "Song not updated."
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
    
    private static string SecondsToTime(int seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(seconds);
        return time.ToString(@"hh\:mm\:ss");
    }
}

public sealed class UpdateSongValidator : AbstractValidator<UpdateSongCommand>
{
    public UpdateSongValidator()
    {
        RuleFor(x => x.SongId)
            .NotEmpty()
            .WithMessage("Song ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Song name is required.");

        RuleFor(x => x.Duration)
            .NotEmpty()
            .WithMessage("Song duration is required.");
    }
}

public sealed class UpdateSongMappingProfile : Profile
{
    public UpdateSongMappingProfile()
    {
        CreateMap<UpdateSongRequest, UpdateSongCommand>();
    }
}

public sealed class UpdateSongRequest
{
    public int SongId { get; init; }
    
    public string Name { get; init; } = default!;
    
    public int Duration { get; init; }
}

public sealed class UpdateSongResponse
{
    public bool IsUpdated { get; set; }
    
    public string Message { get; set; } = default!;
}