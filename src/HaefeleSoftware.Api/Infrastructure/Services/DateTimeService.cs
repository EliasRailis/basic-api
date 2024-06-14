using HaefeleSoftware.Api.Application.Interfaces;

namespace HaefeleSoftware.Api.Infrastructure.Services;

public sealed class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
}