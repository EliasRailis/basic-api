namespace HaefeleSoftware.Api.Domain.Dto;

public sealed class LibraryInformationDto
{
    public int Id { get; init; }

    public string LibraryName { get; init; } = default!;

    public string CreatedAt { get; init; } = default!;
    
    public int AlbumsCount { get; set; }
    
    public IEnumerable<SmallAlbumDto> Albums { get; set; } = [];
}