using d9.utl;
using System.Text.Json.Serialization;

#pragma warning disable IDE0079 // "remove unnecessary suppression": CS8618 is actually necessary lol
#pragma warning disable CS8618 // "must have non-null value": initialized by JSON loading

namespace d9.grp;

internal class GrpConfig
{
    public static readonly GrpConfig Current = Config.TryLoad<GrpConfig>("config.json") ?? throw new Exception("Couldn't find a valid config file!");
    public class ConfigPaths
    {
        [JsonInclude]
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
    public static readonly string GoogleFileId = File.ReadAllText("file id.txt.secret");
}