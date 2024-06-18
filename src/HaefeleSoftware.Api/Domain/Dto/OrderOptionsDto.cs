namespace HaefeleSoftware.Api.Domain.Dto;

public sealed class OrderOptionsDto
{
    public int OrderType { get; }
    
    public int OrderBy { get; }
    
    public OrderOptionsDto(int orderType, int orderBy)
    {
        OrderType = orderType;
        OrderBy = orderBy;
    }
}