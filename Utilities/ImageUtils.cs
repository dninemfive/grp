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
            Func<Image, int> dimensionGetter = direction.IsHorizontal() ? (x) => x.Height : (x) => x.Width;
            int currentTopOrLeftEdge = direction.IsReverse() ? dimensionGetter(result) - dimensionGetter(images.Last()) : 0;
            foreach (Image img in direction.IsReverse() ? images.Reverse() : images)
            {
                result.Mutate((context) => context.DrawImage(img, GetNextPoint(dimensionGetter(result), dimensionGetter(img), currentTopOrLeftEdge, direction), 1));
                currentTopOrLeftEdge += (direction.IsReverse() ? -1 : 1) * (int)(dimensionGetter(img) * (1 - overlap));
            }
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
        public static Point GetNextPoint(int resultDim, int imgDim, int currentEdge, MergeDirection md) => md.IsHorizontal() switch
        {
            true => new(currentEdge, resultDim - imgDim),
            false => new((resultDim - imgDim) / 2, currentEdge)
        };
    }
}
