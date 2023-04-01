using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    public static class ImageUtils
    {
        public static Image Merge(IEnumerable<Image> images, MergeDirection direction = MergeDirection.LeftRight, float overlap = 0.25f)
        {
            Console.WriteLine($"Merge({images.GetType().Name}[{images.Count()}], {direction}, {overlap})");
            Image result = GenerateImageWhichFits(images.Select(x => x.Width), images.Select(x => x.Height), direction);
            if (direction is MergeDirection.TopBottom or MergeDirection.BottomTop)
            {
                if (direction is MergeDirection.TopBottom)
                {
                    int currentTopSide = 0;
                    foreach (Image img in images)
                    {
                        result.Mutate((context) => context.DrawImage(img, new Point((result.Width - img.Width) / 2, currentTopSide), 1));
                        currentTopSide += (int)(img.Height * (1 - overlap));
                    }
                }
                else
                {
                    int currentTopSide = result.Height - images.Last().Height;
                    foreach (Image img in images)
                    {
                        result.Mutate((context) => context.DrawImage(img, new Point((result.Width - img.Width) / 2, currentTopSide), 1));
                        currentTopSide -= (int)(img.Height * (1 - overlap));
                    }
                }
            }
            else if (direction is MergeDirection.LeftRight or MergeDirection.RightLeft)
            {
                if (direction is MergeDirection.LeftRight)
                {
                    int currentLeftSide = 0;
                    foreach (Image img in images)
                    {
                        result.Mutate((context) => context.DrawImage(img, new Point(currentLeftSide, result.Height - img.Height), 1));
                        currentLeftSide += (int)(img.Width * (1 - overlap));
                    }
                }
                else
                {
                    int currentLeftSide = result.Width - images.Last().Width;
                    foreach (Image img in images.Reverse())
                    {
                        result.Mutate((context) => context.DrawImage(img, new Point(currentLeftSide, result.Height - img.Height), 1));
                        currentLeftSide -= (int)(img.Width * (1 - overlap));
                    }
                }
            }
            else throw new ArgumentOutOfRangeException(nameof(direction));
            return result.Autocrop();
        }
        public static Image<Rgba32> GenerateImageWhichFits(IEnumerable<int> widths, IEnumerable<int> heights, MergeDirection direction) => direction switch
        {
            MergeDirection.BottomTop or MergeDirection.TopBottom => new(widths.Max(), heights.Sum()),
            MergeDirection.LeftRight or MergeDirection.RightLeft => new(widths.Sum(), heights.Max()),
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };
        public static bool IsHorizontal(this MergeDirection md) => md is MergeDirection.LeftRight or MergeDirection.RightLeft;
        public static bool IsReverse(this MergeDirection md) => md is MergeDirection.RightLeft or MergeDirection.BottomTop;
    }
}
