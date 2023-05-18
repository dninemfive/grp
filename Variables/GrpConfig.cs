using d9.utl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

#pragma warning disable CS8618
namespace d9.grp
{
    public class GrpConfig
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
            public string OutputFolder = "output";
            [JsonInclude]
            public string DebugFolder = "debug";
            [JsonInclude]
            public string DataFolder = "data";
            [JsonInclude]
            public string WatermarkToSubtract = "images/watermark_subtract.png";
            [JsonInclude]
            public string WatermarkToAdd = "images/watermark_add.png";
            [JsonInclude]
            public string Mask = "images/mask.png";
            [JsonInclude]
            public string? GoogleAuth = null;
        }
        public class Group
        {
            [JsonInclude]
            public string Name;
            [JsonInclude]
            public string GoogleFileId;
            [JsonInclude]
            public string? WatermarkToSubtract;
            [JsonInclude]
            public string? WatermarkToAdd;
            [JsonInclude]
            public string? Mask;
        }
        [JsonInclude]
        // set by jsonserializer
#pragma warning disable CS0649
        public ConfigPaths Paths;
#pragma warning restore CS0649
        [JsonInclude]
        public List<Group> Groups;
        [JsonInclude]
        public bool SkipGoogleDownload = false;
        [JsonInclude]
        public bool SavePerUserImages = false;
        [JsonInclude]
        public bool CopyDescToClipboard = true;
        [JsonInclude]
        public bool SaveDescToFile = true;        
        [JsonInclude]
        public int MaxUsersPerRow = 12;
    }    
}
