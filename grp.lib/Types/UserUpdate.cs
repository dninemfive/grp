namespace d9.grp.lib;

public class UserUpdate
{
    /// <summary>
    /// Discord ID of the user who submitted the update.
    /// </summary>
    public required string UserId { get; set; }
    /// <summary>
    /// Timestamp the update was sent.
    /// </summary>
    public required DateTime Timestamp { get; set; }
    /// <summary>
    /// Guild ID of the server from which the user sent the update. 
    /// <see langword="null"/> only when the update was sent via DM with the bot.
    /// </summary>
    public long? GuildId { get; set; } = null;
    /// <summary>
    /// Whether the data in the update should apply only to the guild from which the update was sent.
    /// If <see cref="GuildId">GuildId</see> is <see langword="null"/>, this is ignored.
    /// </summary>
    public bool GuildSpecific { get; set; } = true;
    public string? ImageUrl { get; set; } = null;
    public string? Height { get; set; } = null;
    public string? PreferredName { get; set; } = null;

    public string? FileName => ImageUrl is not null 
                             ? $"{UserId}{Timestamp:s}{Path.GetExtension(ImageUrl)}".FileNameSafe() 
                             : null;
}