using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8618
namespace grp
{
    internal class Config
    {
        public static Config Current;
        public class ConfigPaths
        {
            public string BaseFolder;
            public string ImageFolder = "/images/";
            public string DebugFolder = "/debug/";
            public string TsvFile = "data.tsv";
            public string WatermarkToSubtract = "watermark_subtract.png";
            public string WatermarkToAdd = "watermark_add.png";
            public string? GoogleAuthKey = null;
        }
        public ConfigPaths Paths;
        public bool Debug = false;
        public bool SavePerUserImages = false;
        public bool CopyDescToClipboard = true;
        public bool SaveDescToFile = false;
        public string? GoogleAuthEmail = null;        
        public string? GoogleFileId = null;
        public int MaxUsersPerRow = 12;
    }
}
