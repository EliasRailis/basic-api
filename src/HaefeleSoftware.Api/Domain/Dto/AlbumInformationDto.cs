namespace HaefeleSoftware.Api.Domain.Dto;

public sealed class AlbumInformationDto
{
    public int AlbumId { get; set; }

    public string AlbumName { get; set; } = default!;

    public string ArtistName { get; set; } = default!;

    public string YearOfRelease { get; set; } = default!;

    public string AlbumDuration { get; set; } = default!;
    
    public int NumberOfSongs { get; set; }

    public IEnumerable<SongInformationDto> Songs { get; set; } = [];
}