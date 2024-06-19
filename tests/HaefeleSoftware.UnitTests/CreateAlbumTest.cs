using System.Net;
using FluentAssertions;
using HaefeleSoftware.Api.Application.Interfaces;
using HaefeleSoftware.Api.Application.Interfaces.Repositories;
using HaefeleSoftware.Api.Domain.Common;
using HaefeleSoftware.Api.Domain.Entities;
using HaefeleSoftware.Api.Domain.Types;
using HaefeleSoftware.Api.Features.Album;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using Serilog;

namespace HaefeleSoftware.UnitTests;

public sealed class CreateAlbumTest
{
    private readonly ILogger _logger = Substitute.For<ILogger>();
    private readonly IAlbumRepository _albumRepository = Substitute.For<IAlbumRepository>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();

    private readonly CreateAlbumCommand _createAlbumCommand = new()
    {
        ArtistId = 3,
        Name = "Album Name",
        YearOfRelease = "2022",
        DurationInSeconds = 3600
    };

    private readonly CurrentUser? _currentUser = new()
    {
        Id = 1,
        Email = "admin@gmail.com"
    };

    [Fact]
    public async Task CreateAlbum_ShouldReturnError_WhenAlbumDoesNotExist()
    {
        // Arrange
        var handler = new CreateAlbumCommandHandler(_logger, _albumRepository, _currentUserService);
        _albumRepository.GetArtistAlbumsByIdAsync(Arg.Any<int>()).ReturnsNull();
        _currentUserService.User.Returns(_currentUser);

        // Act
        var result = await handler.Handle(_createAlbumCommand, new CancellationToken());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsError.Should().BeTrue();

        OnError error = result.Match(null!, err => err);
        error.StatusCode.Should().Match(x => x == HttpStatusCode.NotFound);
        error.Error.Should().Match(x => x == "Artist not found.");
    }

    [Fact]
    public async Task CreateAlbum_ShouldReturnError_WhenAlbumAlreadyExists()
    {
        // Arrange
        var artistResponse = new Artist()
        {
            Id = 1,
            Name = "Artist Name",
            Albums = new List<Album>
            {
                new()
                {
                    Id = 1,
                    Name = "Album Name",
                    YearOfRelease = "2022",
                    Duration = "0h 35m 23s"
                }
            }
        };

        var handler = new CreateAlbumCommandHandler(_logger, _albumRepository, _currentUserService);
        _albumRepository.GetArtistAlbumsByIdAsync(Arg.Any<int>()).Returns(artistResponse);
        _currentUserService.User.Returns(_currentUser);

        // Act
        var result = await handler.Handle(_createAlbumCommand, new CancellationToken());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsError.Should().BeTrue();

        OnError error = result.Match(null!, err => err);
        error.StatusCode.Should().Match(x => x == HttpStatusCode.BadRequest);
        error.Error.Should().Match(x => x == "Album already exists.");
    }

    [Fact]
    public async Task CreateAlbum_ShouldReturnError_WhenExceptionIsThrown()
    {
        // Arrange
        var artistResponse = new Artist
        {
            Id = 1,
            Name = "Artist Name",
            Albums = new List<Album>
            {
                new()
                {
                    Id = 1,
                    Name = "Album Name v1",
                    YearOfRelease = "2022",
                    Duration = "0h 35m 23s"
                }
            }
        };

        var handler = new CreateAlbumCommandHandler(_logger, _albumRepository, _currentUserService);
        _albumRepository.GetArtistAlbumsByIdAsync(Arg.Any<int>()).Returns(artistResponse);
        _currentUserService.User.Returns(_currentUser);
        _albumRepository.AddAlbumAsync(Arg.Any<Album>()).Throws(new Exception("An error occurred."));

        // Act
        var result = await handler.Handle(_createAlbumCommand, new CancellationToken());

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.IsError.Should().BeTrue();

        OnError error = result.Match(null!, err => err);
        error.StatusCode.Should().Match(x => x == HttpStatusCode.BadRequest);
        error.Error.Should().Match(x => x == "An error occurred.");
    }

    [Fact]
    public async Task CreateAlbum_ShouldReturnSuccess_WhenAlbumIsCreated()
    {
        // Arrange
        var artistResponse = new Artist
        {
            Id = 1,
            Name = "Artist Name",
            Albums = new List<Album>
            {
                new()
                {
                    Id = 1,
                    Name = "Album Name v1",
                    YearOfRelease = "2022",
                    Duration = "0h 35m 23s"
                }
            }
        };

        var handler = new CreateAlbumCommandHandler(_logger, _albumRepository, _currentUserService);
        _albumRepository.GetArtistAlbumsByIdAsync(Arg.Any<int>()).Returns(artistResponse);
        _currentUserService.User.Returns(_currentUser);
        _albumRepository.AddAlbumAsync(Arg.Any<Album>()).Returns(true);

        // Act
        var result = await handler.Handle(_createAlbumCommand, new CancellationToken());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.IsError.Should().BeFalse();

        OnSuccess<CreateAlbumResponse> response = result.Match(res => res, null!);
        response.StatusCode.Should().Match(x => x == HttpStatusCode.OK);
        response.Response.Should().NotBeNull();
        response.Response?.IsSuccess.Should().BeTrue();
        response.Response?.AlbumId.Should().Be(0);
        response.Response?.Message.Should().Match(x => x == "Album created.");
    }
}