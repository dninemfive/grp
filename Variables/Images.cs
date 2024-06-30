using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace d9.grp
{
    /// <summary>
    /// Useful data for the watermark removal process, for now.
    /// </summary>
    public static class Images
    {
        /// <summary>
        /// The watermark to add so that the picrew author and the group for whom the image is made are both credited.
        /// </summary>
        public static Image WatermarkToAdd(GrpConfig.Group group) => group.WatermarkToAdd().LoadImage();
        /// <summary>
        /// The image which will be used to mask out the watermarks.
        /// </summary>
        public static Image<Rgba32> WatermarkForSubtraction(GrpConfig.Group group) => group.WatermarkToSubtract().LoadImage().CloneAs<Rgba32>();
        public static readonly Image<Rgba32> DefaultWatermarkForSubtraction = Paths.DefaultWatermarkToSubtract.LoadImage().CloneAs<Rgba32>();
        public static readonly Image AlphaMask = Paths.AlphaMask.LoadImage();
        /// <summary>
        /// The actual pixel coordinates which will be masked out to remove the watermark.
        /// </summary>
        private static HashSet<(int x, int y)>? _watermarkMask = null;
        /// <inheritdoc cref="_watermarkMask"/>
        public static ImmutableHashSet<(int x, int y)> DefaultWatermarkMask
        {
            get
            {
                if (_watermarkMask is null)
                {
                    _watermarkMask = new();
                    using Image<Rgba32> watermarkMask = DefaultWatermarkForSubtraction.CloneAs<Rgba32>();
                    Rgba32 transparent = Color.Transparent;
                    watermarkMask.ProcessPixelRows(accessor =>
                    {
                        for (int y = 0; y < accessor.Height; y++)
                        {
                            Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                            for (int x = 0; x < pixelRow.Length; x++)
                            {
                                if (pixelRow[x].A > 0) _watermarkMask.Add((x, y));
                            }
                        }
                    });
                }
                return _watermarkMask.ToImmutableHashSet();
            }
        }
        private static readonly Dictionary<string, ImmutableHashSet<(int x, int y)>> _watermarkMasks = new();
        public static ImmutableHashSet<(int x, int y)> WatermarkMaskFor(GrpConfig.Group group)
        {
            if (group.WatermarkToSubtract is null) return DefaultWatermarkMask;
            if(!_watermarkMasks.ContainsKey(group.Name))
            {

            } 
        }
    }    
}
