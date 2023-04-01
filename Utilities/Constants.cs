using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    public static class Constants
    {
        public static readonly Image<Rgba32> WatermarkForSubtraction = Paths.WatermarkForSubtraction.LoadImage().CloneAs<Rgba32>();
        private static HashSet<(int x, int y)>? _watermarkMask = null;
        public static ImmutableHashSet<(int x, int y)> WatermarkMask
        {
            get
            {
                if (_watermarkMask is null)
                {
                    _watermarkMask = new();
                    using Image<Rgba32> watermarkMask = WatermarkForSubtraction.CloneAs<Rgba32>();
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
    }
    public static class Paths
    {
        public static readonly string BaseFolder = "C:/Users/dninemfive/Documents/workspaces/misc/grp";
        public static readonly string ImageFolder = Path.Join(BaseFolder, "images");
        public static readonly string DataFile = Path.Join(BaseFolder, "grp responses.tsv");
        public static readonly string WatermarkForSubtraction = Path.Join(BaseFolder, "watermark.png");
        public static readonly string WatermarkForAddition = Path.Join(BaseFolder, "watermark_autocropped.png");
    }
}
