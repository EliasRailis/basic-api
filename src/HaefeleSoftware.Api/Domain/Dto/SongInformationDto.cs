namespace HaefeleSoftware.Api.Domain.Dto;

public sealed class SongInformationDto
{
    public int SongId { get; set; }
    
    public string SongName { get; set; } = default!;
    
    public string SongDuration { get; set; } = default!;
}