using SixLabors.ImageSharp.Processing;

namespace d9.grp.lib.Types;
public class ImageProcessor(string imageFolder, IEnumerable<(int x, int y)> watermarkMask, Image alphaMask)
{
    public readonly string ImageFolder = imageFolder;
    public readonly IEnumerable<(int x, int y)> WatermarkMask = watermarkMask;
    public readonly Image AlphaMask = alphaMask;
    public async Task<long> GetExcessAlpha(Image image)
        => await Task.Run(() => image.MultiplyAlpha(AlphaMask).AlphaSum());
    public async Task<Image> RemoveWatermark(Image image)
        => await Task.Run(() => image.Mask(WatermarkMask));
    public static async Task ResizeForHeight(Image image, Height height)
    {
        await Task.Run(() => image.Mutate((context) => context.Resize(new ResizeOptions()
        {
            Mode = ResizeMode.Stretch,
            Position = AnchorPositionMode.Bottom,
            Size = (Size)(image.Size * height.Ratio)
        })));
    }
    public static async Task PositionInFrame(Image image)
    {
        await Task.Run(() => image.Mutate((context) => context.Resize(new ResizeOptions()
        {
            Mode = ResizeMode.BoxPad,
            Position = AnchorPositionMode.Bottom,
            Size = new((int)(Height.Maximum.Ratio * 600), image.Height)
        })));
    }
    public async Task ProcessImage(ImageInfo info)
    {
        (Image image, UserUpdate update) = info;
        Height height = Height.Parse(update.Height) ?? Height.Default;
        long excessAlpha = await GetExcessAlpha(image);
        image = await RemoveWatermark(image);
        await ResizeForHeight(image, height);
        image = image.Autocrop(AutocropType.Vertical);
        await PositionInFrame(image);
    }
    public async Task<Image?> GetImage(string imageUrl)
    {
        if (ImageUrl is null)
            return null;
        Image result = await NetUtils.DownloadImage(ImageUrl, FileName);
        if (result is null)
            return result;
        // todo: this should be its own method but idk where to put it
        if (result.Width != 600 || result.Height != 600)
        {
            File.Delete(Path.Join(ImageFolder, FileName));
            throw new Exception($"Image at {Url} for user {DiscordId} was not the right size!");
        }
        ExcessAlpha = result.MultiplyAlpha(Images.AlphaMask, DiscordId).AlphaSum();
        result = result.Mask(WatermarkMask);
        result.Mutate((context) => context.Resize(new ResizeOptions()
        {
            Mode = ResizeMode.Stretch,
            Position = AnchorPositionMode.Bottom,
            Size = (Size)(result.Size * Height.Ratio)
        }));
        result = result.Autocrop(AutocropType.Vertical);
        result.Mutate((context) => context.Resize(new ResizeOptions()
        {
            Mode = ResizeMode.BoxPad,
            Position = AnchorPositionMode.Bottom,
            Size = new((int)(Height.Maximum.Ratio * 600), result.Height)
        }));
        if (GrpConfig.Current.SavePerUserImages)
            result.SaveTo(Path.Join(Paths.DebugFolder, FileName));
    }
}
