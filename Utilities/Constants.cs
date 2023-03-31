using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    public static class Constants
    {        
        public static readonly HttpClient Client = new();
        public static readonly Image WatermarkForSubtraction = Utils.LoadImage(Paths.WatermarkForSubtraction);
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
