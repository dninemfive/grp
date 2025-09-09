namespace d9.grp.lib;
/// <summary>
/// Verifies URLs (e.g. checks the domain), downloads, and validates (e.g. dimensions of) images.
/// </summary>
public class ImageDownloader(string imageFolder, string domain, string picrewId)
{
    public bool IsValid(string url)
        => new Uri(url).Host.EndsWith(domain, StringComparison.OrdinalIgnoreCase);
    public bool HasCorrectId(string url)
        => new Uri(url).AbsolutePath.Contains($"image_maker/{picrewId}");
    public bool HasCorrectDimensions(Image image)
        => image.Height == 600 && image.Width == 600;
    public string FileNameFor(string userId, DateTime timestamp, string ext)
        => Path.Join(imageFolder, $"{userId}{timestamp:s}{ext}".FileNameSafe());
    public async Task<(string path, Image image)?> Download(string imageUrl, string userId, DateTime timestamp)
    {
        if (!IsValid(imageUrl) || !HasCorrectId(imageUrl))
            return null;
        string? imagePath = await NetUtils.Download(imageUrl, FileNameFor(userId, timestamp, Path.GetExtension(imageUrl)));
        if (imagePath is null)
            return null;
        Image? image = await Task.Run(() => IoUtils.LoadImage(imagePath));
        if (image is null || !HasCorrectDimensions(image))
            return null;
        return (imagePath, image);
    }
}
