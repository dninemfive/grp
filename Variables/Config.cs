using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

#pragma warning disable CS8618
namespace grp
{
    internal class Config
    {
        public static Config Current;
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
            public string? GoogleAuthKey = null;
            public ConfigPaths() { }
            /*
            [JsonConstructor]
            public ConfigPaths(string baseFolder, string imageFolder, string debugFolder, 
                string tsvFile, string watermarkToSubtract, string watermarkToAdd, string? googleAuthKey)
            {
                BaseFolder = baseFolder;
                ImageFolder = imageFolder;
                DebugFolder = debugFolder;
                TsvFile = tsvFile;
                WatermarkToSubtract = watermarkToSubtract;
                WatermarkToAdd = watermarkToAdd;
                GoogleAuthKey = googleAuthKey;
            }*/
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
        public string? GoogleAuthEmail = null;
        [JsonInclude]
        public string? GoogleFileId = null;
        [JsonInclude]
        public int MaxUsersPerRow = 12;
    }
}
