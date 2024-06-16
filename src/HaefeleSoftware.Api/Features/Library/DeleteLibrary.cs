using System.Net;
using FluentValidation;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Types;
using MediatR;
using Serilog;

namespace HaefeleSoftware.Api.Features.Library;

public sealed class DeleteLibraryEndpoint : IEndpoint
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapDelete("library/delete/{id:int}", async (int id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteLibraryCommand(id));
            return result.Match(Results.Ok, Results.BadRequest);
        })
        .MapToApiVersion(1)
        .RequireAuthorization(IdentityRoles.Customer);
    }
}

public sealed class DeleteLibraryCommand : IRequest<Result<OnSuccess<DeleteLibraryResponse>, OnError>>
{
    public int LibraryId { get; } 
    
    public DeleteLibraryCommand(int libraryId)
    {
        LibraryId = libraryId;
    }
}

public sealed class DeleteLibraryCommandHandler : IRequestHandler<DeleteLibraryCommand,
    Result<OnSuccess<DeleteLibraryResponse>, OnError>>
{
    private readonly ILogger _logger;
    private readonly ILibraryRepository _libraryRepository;
    private readonly CurrentUser? _currentUser;

    public DeleteLibraryCommandHandler(ILogger logger, ILibraryRepository libraryRepository, 
        ICurrentUserService currentUser)
    {
        _logger = logger;
        _libraryRepository = libraryRepository;
        _currentUser = currentUser.User;
    }

    public async Task<Result<OnSuccess<DeleteLibraryResponse>, OnError>> Handle(DeleteLibraryCommand request, 
        CancellationToken cancellationToken)
    {
        try
        {
            Domain.Entities.Library? library = await _libraryRepository
                .GetLibraryByIdAsync(request.LibraryId);
            
            if (library is null)
            {
                return new OnError(HttpStatusCode.NotFound, "Library not found.");
            }
            
            library.IsDeleted = true;
            library.LastModifiedBy = _currentUser?.Email;
            bool updated = await _libraryRepository.UpdateLibraryAsync(library);
            
            return new OnSuccess<DeleteLibraryResponse>
            {
                StatusCode = HttpStatusCode.OK,
                Response = new DeleteLibraryResponse
                {
                    IsDeleted = updated,
                    Message = updated ? "Library deleted." : "Library not deleted."
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

public sealed class DeleteLibraryValidator : AbstractValidator<DeleteLibraryCommand>
{
    public DeleteLibraryValidator()
    {
        RuleFor(x => x.LibraryId)
            .NotEmpty()
            .WithMessage("LibraryId is required.");
    }
}

public sealed class DeleteLibraryResponse
{
    public bool IsDeleted { get; set; }

    public string Message { get; set; } = default!;
}