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
    internal class Config
    {
        public static Config Current;
        public static void Load(string path)
        {
            Config? config = JsonSerializer.Deserialize<Config>(File.ReadAllText(path));
            if (config is null) throw new Exception($"Could not find config file at path `{path}`!");
            Current = config;
        }
        public class ConfigPaths
        {            
            [JsonInclude]
            public string BaseFolder;
            [JsonInclude]
            public string ImageFolder = "/images/";
            [JsonInclude]
            public string DebugFolder = "/debug/";
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
        public ConfigPaths Paths;
        [JsonInclude]
        public bool Debug = false;
        [JsonInclude]
        public bool SavePerUserImages = false;
        [JsonInclude]
        public bool CopyDescToClipboard = true;
        [JsonInclude]
        public bool SaveDescToFile = false;        
        [JsonInclude]
        public int MaxUsersPerRow = 12;
    }
    public class GoogleAuthConfig
    {
        [JsonInclude]
        public string AuthKey;
        [JsonInclude]
        public string AuthEmail;
        [JsonInclude]
        public string FileId;
        public GoogleAuthConfig(string authkey, string email, string fileid)
        {
            AuthKey = authkey;
            AuthEmail = email;
            FileId = fileid;
        }
    }
}
