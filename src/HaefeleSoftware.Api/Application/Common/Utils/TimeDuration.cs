﻿namespace HaefeleSoftware.Api.Application.Common.Utils;

public static class TimeDuration
{
    public static string Format(int seconds)
    {
        var timeSpan = TimeSpan.FromSeconds(seconds);
        return timeSpan.ToString(@"hh\:mm\:ss");
    }
}