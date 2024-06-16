using System.Net;
using FluentValidation;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Song;

public sealed class DeleteSongEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("songs/delete/{id:int}", async (int id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteSongCommand(id));
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Admin);
    }
}

public sealed class DeleteSongCommand : IRequest<Result<OnSuccess<DeleteSongResponse>, OnError>>
{
    public int SongId { get; }

    public DeleteSongCommand(int songId)
    {
        SongId = songId;
    }
}

public sealed class DeleteSongCommandHandler : IRequestHandler<DeleteSongCommand,
    Result<OnSuccess<DeleteSongResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly ISongRepository _songRepository;
    private readonly CurrentUser? _currentUser;

    public DeleteSongCommandHandler(ILogger logger, ISongRepository songRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _songRepository = songRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<DeleteSongResponse>, OnError>> Handle(DeleteSongCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.Song? song = await _songRepository.GetSongByIdAsync(request.SongId);
            
            if (song is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Song not found.");
            }
            
            song.IsDeleted = true;
            song.LastModifiedBy = _currentUser?.Email;
            bool updated = await _songRepository.UpdateSongAsync(song);
            
            return new OnSuccess<DeleteSongResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new DeleteSongResponse
                {
                    IsDeleted = updated,
                    Message = updated ? "Song deleted." : "Song not deleted."
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

public sealed class DeleteSongValidator : AbstractValidator<DeleteSongCommand>
{
    public DeleteSongValidator()
    {
        RuleFor(x => x.SongId)
            .NotEmpty()
            .WithMessage("Song ID is required.");
    }
}

public sealed class DeleteSongResponse
{
    public bool IsDeleted { get; set; }
    
    public string Message { get; set; } = default!;
}