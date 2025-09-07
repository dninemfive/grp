using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace d9.grp.lib;

public class UserUpdate
{
    public required string DiscordId { get; set; }
    public required DateTime Timestamp { get; set; }
    public long? GuildId { get; set; } = null;
    public string? ImageUrl { get; set; } = null;
    public string? Height { get; set; } = null;
    public string? PreferredName { get; set; } = null;

    public string? FileName => ImageUrl is not null 
                             ? $"{DiscordId}{Timestamp:s}{Path.GetExtension(ImageUrl)}".FileNameSafe() 
                             : null;
}