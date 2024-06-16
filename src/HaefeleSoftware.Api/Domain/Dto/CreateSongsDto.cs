namespace HaefeleSoftware.Api.Domain.Dto;

public sealed class CreateSongsDto
{
    public string Name { get; set; } = default!;
    
    public int Duration { get; set; }
}