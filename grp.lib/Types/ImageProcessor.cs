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
    public async Task<UserImage> Process(User user, Image? image)
    {
        image ??= IoUtils.LoadImage(user.ImagePath);
        Height height = user.Height ?? Height.Default;
        long excessAlpha = await GetExcessAlpha(image);
        image = await RemoveWatermark(image);
        await ResizeForHeight(image, height);
        image = image.Autocrop(AutocropType.Vertical);
        await PositionInFrame(image);
        return new(user, image, excessAlpha);
    }
}
