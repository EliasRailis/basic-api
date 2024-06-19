using System.Net;
using FluentValidation;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Dto;
using HaefeleSoftware.Api.Domain.Enums;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace HaefeleSoftware.Api.Features.Album;

public sealed class GetAlbumsEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("albums", async ([FromQuery] int? orderType, [FromQuery] int? orderBy, IMediator mediator) =>
        {
            var result = await mediator.Send(new GetAlbumsQuery(orderType, orderBy));
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization();
    }
}

public sealed class GetAlbumsQuery : IRequest<Result<OnSuccess<GetAlbumsResponse>, OnError>>
{
    public int? OrderType { get; }
    
    public int? OrderBy { get; }

    public GetAlbumsQuery(int? orderType, int? orderBy)
    {
        OrderType = orderType;
        OrderBy = orderBy;
    }
}

public sealed class GetAlbumsQueryHandler : IRequestHandler<GetAlbumsQuery,
    Result<OnSuccess<GetAlbumsResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly IAlbumRepository _albumRepository;

    public GetAlbumsQueryHandler(ILogger logger, IAlbumRepository albumRepository)
    {
        _logger = logger;
        _albumRepository = albumRepository;
    }

    public async Task<Result<OnSuccess<GetAlbumsResponse>, OnError>> Handle(GetAlbumsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<Domain.Entities.Album> albums = await _albumRepository.GetAllActiveAlbumsAsync();

            var albumsInformationDto = new List<AlbumInformationDto>();
            foreach (var album in albums)
            {
                List<SongInformationDto> songsInformationDto = AlbumSongs(album.Songs);
                if (songsInformationDto.Count == 0) continue;

                albumsInformationDto.Add(new AlbumInformationDto
                {
                    AlbumId = album.Id,
                    AlbumName = album.Name,
                    ArtistName = album.Artist.Name,
                    YearOfRelease = album.YearOfRelease,
                    AlbumDuration = album.Duration,
                    NumberOfSongs = album.NumberOfSongs,
                    Songs = songsInformationDto
                });
            }

            if (request.OrderType is not null && request.OrderBy is not null)
            {
                var options = new OrderOptionsDto((int)request.OrderType, (int)request.OrderBy);
                OrderResults(ref albumsInformationDto, options);
            }

            return new OnSuccess<GetAlbumsResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new GetAlbumsResponse(albumsInformationDto)
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

    private static void OrderResults(ref List<AlbumInformationDto> albums, OrderOptionsDto options)
    {
        albums = options.OrderType switch
        {
            (int)OrderType.YearOfRelease when options.OrderBy is (int)OrderBy.Ascending => 
                albums.OrderBy(x => int.Parse(x.YearOfRelease)).ToList(),
            (int)OrderType.YearOfRelease when options.OrderBy is (int)OrderBy.Descending => 
                albums.OrderByDescending(x => int.Parse(x.YearOfRelease)).ToList(),
            (int)OrderType.NumberOfSongs when options.OrderBy is (int)OrderBy.Ascending => 
                albums.OrderBy(x => x.NumberOfSongs).ToList(),
            (int)OrderType.NumberOfSongs when options.OrderBy is (int)OrderBy.Descending=> 
                albums.OrderByDescending(x => x.NumberOfSongs).ToList(),
            (int)OrderType.Alphabetical when options.OrderBy is (int)OrderBy.Ascending => 
                albums.OrderBy(x => x.AlbumName).ToList(),
            (int)OrderType.Alphabetical when options.OrderBy is (int)OrderBy.Descending => 
                albums.OrderByDescending(x => x.AlbumName).ToList(),
            _ => albums
        };
    }

    private static List<SongInformationDto> AlbumSongs(IEnumerable<Domain.Entities.Song> songs)
    {
        return songs.Where(x => !x.IsDeleted)
            .Select(song => new SongInformationDto
            {
                SongId = song.Id,
                SongName = song.Name,
                SongDuration = song.Duration
            }).ToList();
    }
}

public sealed class GetAlbumsValidator : AbstractValidator<GetAlbumsQuery>
{
    public GetAlbumsValidator()
    {
        RuleFor(x => x.OrderType)
            .InclusiveBetween(1, 3)
            .WithMessage("Invalid OrderType value.");
        
        RuleFor(x => x.OrderBy)
            .InclusiveBetween(1, 2)
            .WithMessage("Invalid OrderBy value.");
    }
}

public sealed class GetAlbumsResponse
{
    public IEnumerable<AlbumInformationDto> Albums { get; }

    public GetAlbumsResponse(IEnumerable<AlbumInformationDto> albums)
    {
        Albums = albums;
    }
}