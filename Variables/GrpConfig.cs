using d9.utl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

#pragma warning disable CS8618
namespace grp
{
    internal class GrpConfig
    {
        public static readonly GrpConfig Current = Config.TryLoad<GrpConfig>("config.json") ?? throw new Exception("Couldn't find a valid config file!");
        public class ConfigPaths
        {
            [JsonInclude]
            // set by jsonserializer
#pragma warning disable CS0649
            public string BaseFolder;
#pragma warning restore CS0649
            [JsonInclude]
            public string ImageFolder = "images";
            [JsonInclude]
            public string DebugFolder = "debug";
            [JsonInclude]
            public string TsvFile = "data.tsv";
            [JsonInclude]
            public string WatermarkToSubtract = "watermark_subtract.png";
            [JsonInclude]
            public string WatermarkToAdd = "watermark_add.png";
            [JsonInclude]
            public string? GoogleAuth = null;
        }
        [JsonInclude]
        // set by jsonserializer
#pragma warning disable CS0649
        public ConfigPaths Paths;
#pragma warning restore CS0649
        [JsonInclude]
        public bool Debug = false;
        [JsonInclude]
        public bool SkipGoogleDownload = false;
        [JsonInclude]
        public bool SavePerUserImages = false;
        [JsonInclude]
        public bool CopyDescToClipboard = true;
        [JsonInclude]
        public bool SaveDescToFile = false;        
        [JsonInclude]
        public int MaxUsersPerRow = 12;
        [JsonIgnore]
        public static string GoogleFileId = "";
    }    
}
