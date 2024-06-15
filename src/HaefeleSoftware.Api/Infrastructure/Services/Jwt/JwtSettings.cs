﻿namespace HaefeleSoftware.Api.Infrastructure.Services.Jwt;

public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";
    
    public string AccessTokenSecret { get; init; } = string.Empty;
    
    public string RefreshTokenSecret { get; init; } = string.Empty;
    
    public string Issuer { get; init; } = string.Empty;
    
    public string Audience { get; init; } = string.Empty;
}