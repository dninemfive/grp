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
        public ConfigPaths Paths;
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
        private GoogleAuthConfig? _googleAuth = null;
        [JsonIgnore]
        public GoogleAuthConfig? GoogleAuth
        {
            get
            {
                if (Paths.GoogleAuth is null) return null;
                _googleAuth ??= JsonSerializer.Deserialize<GoogleAuthConfig>(File.ReadAllText(Paths.GoogleAuth));
                return _googleAuth;
            }
        }
    }
    public class GoogleAuthConfig
    {
        [JsonInclude]
        public string Key;
        [JsonInclude]
        public string Email;
        [JsonInclude]
        public string FileId;
        public GoogleAuthConfig(string key, string email, string fileId)
        {
            Key = key;
            Email = email;
            FileId = fileId;
        }
    }
}
