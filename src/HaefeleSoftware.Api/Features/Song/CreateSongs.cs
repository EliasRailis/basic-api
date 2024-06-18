using System.Net;
using AutoMapper;
using FluentValidation;
using HaefeleSoftware.Api.Application.Common.Utils;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Dto;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Song;

public sealed class CreateSongsEndpoint : IEndpoint
{
    private readonly IMapper _mapper;

    public CreateSongsEndpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("songs/create", async (CreateSongsRequest request, ISender sender) =>
        {
            var command = _mapper.Map<CreateSongsCommand>(request);
            var result = await sender.Send(command);
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Admin);
    }
}

public sealed class CreateSongsCommand : IRequest<Result<OnSuccess<CreateSongsResponse>, OnError>>
{
    public int AlbumId { get; init; }
    
    public List<CreateSongsDto> Songs { get; init; } = [];
}

public sealed class CreateSongsCommandHandler : IRequestHandler<CreateSongsCommand,
    Result<OnSuccess<CreateSongsResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly ISongRepository _songRepository;
    private readonly IAlbumRepository _albumRepository;
    private readonly CurrentUser? _currentUser;

    public CreateSongsCommandHandler(ILogger logger, ISongRepository songRepository, 
        ICurrentUserService currentUser, IAlbumRepository albumRepository)
    {
        _logger = logger;
        _songRepository = songRepository;
        _albumRepository = albumRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<CreateSongsResponse>, OnError>> Handle(CreateSongsCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.Album? albumSongs = await _songRepository.GetAlbumSongsAsync(request.AlbumId);
            
            if (albumSongs is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Album not found.");
            }

            var newSongs = new List<Domain.Entities.Song>();
            foreach (var song in request.Songs)
            {
                if (albumSongs.Songs.Where(x => !x.IsDeleted).Any(x => x.Name == song.Name.Trim()))
                {
                    continue;
                }
                
                newSongs.Add(new Domain.Entities.Song
                {
                    Name = song.Name.Trim(),
                    Duration = TimeDuration.Format(song.Duration),
                    IsDeleted = false,
                    FK_AlbumId = request.AlbumId,
                    CreatedBy = _currentUser?.Email!
                });
                
                albumSongs.NumberOfSongs++;
            }
            
            bool created = await _songRepository.AddSongsAsync(newSongs);

            if (created)
            {
                albumSongs.LastModifiedBy = _currentUser?.Email;
                await _albumRepository.UpdateAlbumAsync(albumSongs);
            }
            
            return new OnSuccess<CreateSongsResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new CreateSongsResponse
                {
                    IsSuccess = created,
                    Message = created ? "Songs created successfully." : "Failed to create songs.",
                    SongsIds = albumSongs.Songs.Select(x => x.Id).ToList()
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

public sealed class CreateSongsValidator : AbstractValidator<CreateSongsCommand>
{
    public CreateSongsValidator()
    {
        RuleFor(x => x.AlbumId)
            .NotEmpty()
            .WithMessage("AlbumId is required.");

        RuleForEach(x => x.Songs).SetValidator(new SongsDtoValidator());
    }

    private class SongsDtoValidator : AbstractValidator<CreateSongsDto>
    {
        public SongsDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Duration)
                .NotEmpty()
                .WithMessage("Duration is required.")
                .GreaterThan(0)
                .WithMessage("Duration must be greater than 0.");
        }
    }
}

public sealed class CreateSongsMappingProfile : Profile
{
    public CreateSongsMappingProfile()
    {
        CreateMap<CreateSongsRequest, CreateSongsCommand>();
    }
}

public sealed class CreateSongsRequest
{
    public int AlbumId { get; init; }
    
    public List<CreateSongsDto> Songs { get; init; } = [];
}

public sealed class CreateSongsResponse
{
    public bool IsSuccess { get; set; }
    
    public string Message { get; set; } = default!;

    public List<int> SongsIds { get; set; } = [];
}