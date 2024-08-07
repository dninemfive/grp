﻿using d9.utl;
using SixLabors.ImageSharp.Processing;
using System.Globalization;

namespace d9.grp;

/// <summary>
/// Represents a user constructed from a <see cref="TsvRow">row</see> of a <see cref="TsvDocument"/>.
/// </summary>
internal class User
{
    /// <summary>
    /// The timestamp associated with the <see cref="TsvRow">row</see> of the <see
    /// cref="TsvDocument"/> used to construct this user.
    /// </summary>
    public DateTime Timestamp { get; private set; }
    /// <summary>
    /// The user's canonical discord ID, as it shows up in Discord proper.
    /// </summary>
    public string DiscordId { get; private set; }
    /// <summary>
    /// The user's preferred name when describing the group photo.
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// The image downloaded from the url listed in this user's <see cref="TsvRow">row</see>.
    /// </summary>
    public Image? Image { get; private set; }
    /// <summary>
    /// The user's parsed height.
    /// </summary>
    public Height Height { get; private set; }
    /// <summary>
    /// The url listed in this user's <see cref="TsvRow">row</see>.
    /// </summary>
    private string Url { get; set; }
    /// <summary>
    /// Constructs a user from a <see cref="TsvRow">row</see>.
    /// </summary>
    /// <remarks>
    /// In order to successfully construct a user, the row must have the following columns, in the
    /// following order:
    /// <list type="number">
    /// <item><c>timestamp</c> (key)</item>
    /// <item><c>discord id</c></item>
    /// <item><c>display name</c> (nullable)</item>
    /// <item><c>url</c></item>
    /// <item><c>height</c></item>
    /// </list>
    /// </remarks>
    /// <param name="row">The <see cref="TsvRow"/> containing the data to parse.</param>
    private User(TsvRow row)
    {
        Timestamp = DateTime.ParseExact(row["timestamp"]!.Trim(), "M/d/yyyy H:mm:ss", CultureInfo.InvariantCulture);
        DiscordId = row["discord id"]!;
        Name = !string.IsNullOrEmpty(row["display name"]?.Trim()) ? row["display name"]! : DiscordId;
        Url = row["url"]!;
        Height = Height.Parse(row["height"]!).Clamp(Height.Minimum, Height.Maximum);
    }
    /// <summary>
    /// Downloads the image associated with this user.
    /// </summary>
    /// <returns><see langword="void"/>.</returns>
    private async Task GetImage()
    {
        Image = await NetUtils.DownloadImage(Url, FileName);
        if (Image is null)
            return;
        if (Image.Width != 600 || Image.Height != 600)
        {
            File.Delete(Path.Join(Paths.ImageFolder, FileName));
            throw new Exception($"Image at {Url} for user {DiscordId} was not the right size!");
        }
        ExcessAlpha = Image.MultiplyAlpha(Images.AlphaMask, DiscordId).AlphaSum();
        Image = Image.Mask(Images.WatermarkMask);
        Image.Mutate((context) => context.Resize(new ResizeOptions()
        {
            Mode = ResizeMode.Stretch,
            Position = AnchorPositionMode.Bottom,
            Size = (Size)(Image.Size * Height.Ratio)
        }));
        Image = Image.Autocrop(AutocropType.Vertical);
        Image.Mutate((context) => context.Resize(new ResizeOptions()
        {
            Mode = ResizeMode.BoxPad,
            Position = AnchorPositionMode.Bottom,
            Size = new((int)(Height.Maximum.Ratio * 600), Image.Height)
        }));
        if (GrpConfig.Current.SavePerUserImages)
            Image.SaveTo(Path.Join(Paths.DebugFolder, FileName));
    }
    /// <summary>
    /// <inheritdoc cref="User(TsvRow)" path="/summary"/>
    /// </summary>
    /// <remarks>
    /// <inheritdoc cref="User(TsvRow)" path="/remarks"/><br/><br/> Exists because downloading the
    /// image must be <see langword="async"/>, and normal constructors can't support that.
    /// </remarks>
    /// <param name="row"><inheritdoc cref="User(TsvRow)" path="/param[@name='row']"/></param>
    /// <returns>The <see cref="User"/> parsed from the given <see cref="TsvRow"/>.</returns>
    public static async Task<User> Parse(TsvRow row)
    {
        User result = new(row);
        await result.GetImage();
        return result;
    }
    /// <summary>
    /// The filename which will be given to this user's image when saved.
    /// </summary>
    public string FileName => $"{DiscordId}{Path.GetExtension(Url)}";
    public long ExcessAlpha { get; private set; } = -1;
    public override string ToString() => $"{Name} ({Timestamp:g}) ";
}