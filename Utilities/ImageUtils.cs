using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    /// <summary>
    /// Utilities related to processing <see href="https://docs.sixlabors.com/articles/imagesharp/index.html">ImageSharp</see> <see cref="Image"/>s.
    /// </summary>
    public static class ImageUtils
    {
        /// <summary>
        /// Merges the specified <c>images</c> in the specified <c>direction</c> with the specified <c>overlap</c>.
        /// </summary>
        /// <param name="images">The <see cref="Image"/>s to merge.</param>
        /// <param name="direction">Which <see cref="MergeDirection">direction</see> to merge the images in:
        /// <list type="bullet">
        /// <item><see cref="MergeDirection.LeftRight"/> places the leftmost image first, and therefore images will overlay those to their left;</item>
        /// <item><see cref="MergeDirection.RightLeft"/> places the rightmost image first, and therefore images will overlay those to their right;</item>
        /// <item><see cref="MergeDirection.TopBottom"/> places the topmost image first, and therefore images will overlay those above them vertically;</item>
        /// <item><see cref="MergeDirection.RightLeft"/> places the bottommost image first, and therefore images will overlay those below them vertically.</item> 
        /// </list>Note that the images will be in the same order, this merely determines which images overlay which others.</param>
        /// <param name="overlap">How much images should overlap. The default value, <c>0.25</c>, indicates that each image will overlay 25% of the image previous.
        /// <br/> A value of <c>0</c> means no overlap, and a value of <c>1</c> means complete overlap. Values below 0 or above 1 are undefined but the former
        /// should work, increasing image spacing.</param>
        /// <returns>An <see cref="Image"/> consisting of the specified <c>images</c> overlaid as described above.</returns>
        public static Image Merge(this IEnumerable<Image> images, MergeDirection direction = MergeDirection.LeftRight, float overlap = 0.25f)
        {
            if (images.Count() == 1) return images.First();
            Image result = GenerateImageWhichFits(images.Select(x => x.Width), images.Select(x => x.Height), direction, overlap);
            // edgeDimension is used for figuring out the left or top edge
            // alignDimension is used for aligning the image
            Func<Image, int> edgeDimension = direction.IsHorizontal() ? (x) => x.Width : (x) => x.Height,
                             alignDimension = direction.IsHorizontal() ? (x) => x.Height : (x) => x.Width;
            int currentTopOrLeftEdge = direction.IsReverse() ? edgeDimension(result) : 0;
            bool first = true;
            foreach (Image img in direction.IsReverse() ? images.Reverse() : images)
            {
                if (direction.IsReverse()) currentTopOrLeftEdge -= (int)(edgeDimension(img) * (first ? 1 : (1 - overlap)));
                result.Mutate((context) => context.DrawImage(img, GetEdgePoint(alignDimension(result), alignDimension(img), currentTopOrLeftEdge, direction), 1));
                if(!direction.IsReverse()) currentTopOrLeftEdge += (int)(edgeDimension(img) * (1 - overlap));
                first = false;
            }
            return result.Autocrop();
        }
        /// <summary>
        /// Generates an image which fits a merged image; i.e. it sums the dimension in which the images will be overlaid, and takes the maximum of the other dimension.
        /// </summary>
        /// <param name="widths">The <see cref="Image.Width">width</see>s of the images which will be merged.</param>
        /// <param name="heights">The <see cref="Image.Height">height</see>s of the images which will be merged.</param>
        /// <param name="direction">The <see cref="MergeDirection">direction</see> in which the images will be merged.</param>
        /// <returns>An <see cref="Image"/>&lt;<see cref="Rgba32"/>> with dimensions which fit a merged image, as described above.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown only if you pass an <see langword="int"/> outside the valid values of <see cref="MergeDirection"/>
        /// cast to a <see cref="MergeDirection"/>, so don't do that.</exception>
        public static Image<Rgba32> GenerateImageWhichFits(IEnumerable<int> widths, IEnumerable<int> heights, MergeDirection direction, float overlap) => direction switch
        {
            MergeDirection.BottomTop or MergeDirection.TopBottom => new(widths.Max(), 
                heights.Select(x => (int)(x * (1 - overlap))).Sum() + (int)MiscUtils.Mean(heights.First(), heights.Last())),
            MergeDirection.LeftRight or MergeDirection.RightLeft => new(widths.Select(x => (int)(x * (1 - overlap) 
            + (int)MiscUtils.Mean(widths.First(), widths.Last()))).Sum(), heights.Max()),
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
        /// <summary>
        /// Whether or not a <see cref="MergeDirection"/> is either left-to-right or right-to-left.
        /// </summary>
        /// <param name="md">The <see cref="MergeDirection"/> whose direction to check.</param>
        /// <returns><see langword="true"/> if the merge direction is either <see cref="MergeDirection.LeftRight"/> or <see cref="MergeDirection.RightLeft"/>,
        /// or <see langword="false"/> otherwise.</returns>
        public static bool IsHorizontal(this MergeDirection md) => md is MergeDirection.LeftRight or MergeDirection.RightLeft;
        /// <summary>
        /// Whether or not a <see cref="MergeDirection"/> is processed in reverse relative to the normal <see cref="Image"/> coordinates.
        /// </summary>
        /// <remarks>For example, moving left to right within image coordinates starts at 0 and counts up to the image's width; this is defined as "normal" 
        /// (or more precisely non-reverse) behavior. Conversely, moving right to left starts at the image's width and ends at 0, so this is the "reverse" 
        /// of "normal."</remarks>
        /// <param name="md">The <see cref="MergeDirection"/> whose reverse status to check.</param>
        /// <returns><see langword="true"/> if the merge direction must be processed in reverse as described above, or <see langword="false"/> otherwise.</returns>
        public static bool IsReverse(this MergeDirection md) => md is MergeDirection.RightLeft or MergeDirection.BottomTop;
        /// <summary>
        /// Gets the point describing the edge and alignment to place an image at when merging.
        /// </summary>
        /// <param name="resultDim">The width or height, depending on the merge direction, of the canvas being used to merge images.</param>
        /// <param name="imgDim">The width or height, depending on the merge direction, of the image being painted onto the canvas.</param>
        /// <param name="currentEdge">The current left or top edge, depending on the merge direction.</param>
        /// <param name="md">The direction of the merge.</param>
        /// <returns>If the merge direction <see cref="IsHorizontal">is horizontal</see>, a point which would place the image along the specified
        /// left hand side and aligned to the bottom of the canvas; otherwise, a point which would place it along the specified top edge and
        /// center it horizontally.</returns>
        private static Point GetEdgePoint(int resultDim, int imgDim, int currentEdge, MergeDirection md) => md.IsHorizontal() switch
        {
            true => new(currentEdge, resultDim - imgDim),
            false => new((resultDim - imgDim) / 2, currentEdge)
        };
        /// <summary>
        /// Makes <see cref="Rgba32"/> pixels at specified points in an <see cref="Image"/> transparent, without modifying the original.
        /// </summary>
        /// <param name="image">The image whose pixels will be made transparent.</param>
        /// <param name="points">The coordinates of the pixels to be made transparent.</param>
        /// <returns>An <see cref="Image"/>&lt;<see cref="Rgba32"/>> corresponding to the original <c>image</c>, but with the specified points deleted.</returns>
        public static Image Mask(this Image image, IEnumerable<(int x, int y)> points)
        {
            Image<Rgba32> rgbImage = image.CloneAs<Rgba32>();
            foreach ((int x, int y) in points) rgbImage[x, y] = Color.Transparent;
            return rgbImage;
        }
        /// <summary>
        /// Crops any transparency on the outside of an image, leaving only the minimal region required to contain the non-transparent pixels,
        /// without modifying the original.
        /// </summary>
        /// <param name="image">The image to crop.</param>
        /// <param name="type">Whether to crop only the <see cref="AutocropType.Horizontal">left and right</see> parts of the image, the 
        /// <see cref="AutocropType.Vertical">top and bottom</see> thereof, or <see cref="AutocropType.Both">both</see>.</param>
        /// <returns>The original <c>image</c>, autocropped as described above.</returns>
        public static Image Autocrop(this Image image, AutocropType type = AutocropType.Both)
        {
            Image<Rgba32> rgbImg = image.CloneAs<Rgba32>();
            List<(int min, int max)> rowBounds = new();
            List<bool> rowEmptiness = new();
            rgbImg.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    bool encounteredNonEmptyPixel = false;
                    int minX = 0, maxX = 0;
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                    for (int x = 0; x < pixelRow.Length; x++)
                    {
                        Rgba32 pixel = pixelRow[x];
                        if (pixel.IsEmpty())
                        {
                            if (!encounteredNonEmptyPixel) minX = x;
                            else if (!pixelRow[x - 1].IsEmpty()) maxX = x;
                        }
                        else
                        {
                            encounteredNonEmptyPixel = true;
                        }
                    }
                    rowBounds.Add((minX, maxX));
                    rowEmptiness.Add(!encounteredNonEmptyPixel);
                }
            });
            int minX = type switch
            {
                AutocropType.Vertical => 0,
                _ => rowBounds.Select(x => x.min).Min()
            };
            int maxX = type switch
            {
                AutocropType.Vertical => rgbImg.Width,
                _ => rowBounds.Select(x => x.max).Max()
            };
            int minY = 0, maxY = rgbImg.Height;
            if (type is not AutocropType.Horizontal)
            {
                foreach (bool b in rowEmptiness.TakeWhile(x => x == true)) minY++;
                foreach (bool b in (rowEmptiness as IEnumerable<bool>).Reverse().TakeWhile(x => x == true)) maxY--;
            }
            Rectangle rect = new(minX, minY, maxX - minX, maxY - minY);
            rgbImg.Mutate((context) => context.Crop(rect));
            return rgbImg;
        }
        /// <summary>
        /// Whether or not a given <see cref="Rgba32"/> pixel is fully transparent.
        /// </summary>
        /// <param name="pixel">The pixel to check for full transparency.</param>
        /// <returns><see langword="true"/> if the pixel is fully transparent, i.e. has an opacity of 0, or <see langword="false"/> otherwise.</returns>
        public static bool IsEmpty(this Rgba32 pixel) => pixel.A == 0;
        public static long AlphaSum(this Image image)
        {
            Image<Rgba32> rgbImage = image.CloneAs<Rgba32>();
            long result = 0;
            rgbImage.ProcessPixelRows(accessor =>
            {
                for(int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                    for (int x = 0; x < pixelRow.Length; x++) result += pixelRow[x].A;
                }
            });
            return result;
        }
        public static Image MultiplyAlpha(this Image a, Image b, string? debugUsername = null)
        {
            if (a.Width != b.Width || a.Height != b.Height) throw new NotImplementedException();
            Image<Rgba32> rgb_a = a.CloneAs<Rgba32>(),
                          rgb_b = b.CloneAs<Rgba32>();
            Image<Rgba32> result = new(a.Width, a.Height);
            for(int x = 0; x < a.Width; x++)
            {
                for(int y = 0; y < a.Height; y++)
                {
                    result[x, y] = rgb_a[x, y].MultiplyAlpha(rgb_b[x, y]);
                }
            }
            result.SaveTo(StringUtils.DebugName($"MultiplyAlpha{debugUsername ?? ""}", result.GetHashCode()));
            return result;
        }
        public static Rgba32 MultiplyAlpha(this Rgba32 a, Rgba32 b)
        {
            byte alpha = (byte)(a.A.ToFloat() * b.A.ToFloat() * byte.MaxValue);
            return new(255, 255, 255, alpha);
        }
        public static float ToFloat(this byte b) => b / byte.MaxValue;
    }
}
