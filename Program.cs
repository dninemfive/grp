using grp;
using System.Net.Http;
using System.Text.Json;
using System.Xml.Schema;
using System.Windows;
using System.Runtime.InteropServices;

#region load config
string? configPath = null;
if (args.Length > 1)
{
    string firstArg = args.First();
    if (!File.Exists(firstArg))
    {
        Console.WriteLine($"Could not find config file at `{firstArg}`! Defaulting to `{Paths.DefaultConfigFile}`.");
    }
    else configPath = firstArg;
}
Config.Load(configPath ?? Paths.DefaultConfigFile);
#endregion load config
#region prepare database
Paths.CreateFolders();
if(Config.Current.GoogleAuth is not null) NetUtils.DownloadTsv(Config.Current.GoogleAuth.FileId, Paths.TsvFile);
List<string> rawTsv = File.ReadAllLines(Paths.TsvFile).Skip(1).Where(x => !string.IsNullOrEmpty(x?.Trim())).ToList();
ColumnInfoSet columns = new(
    ("timestamp", 24, ColumnType.Key),
    ("discord id", 42),
    ("display name", 32, ColumnType.Nullable),
    ("url", 128),
    ("height", 8)
);
TsvDocument document = new(columns, rawTsv);
foreach (string s in document.Readable) Console.WriteLine(s);
#endregion prepare database
#region load users
List<User> users = new();
foreach (TsvRow row in document.Rows)
{
    users.Add(await User.Parse(row));
}
List<User> latestUniqueUsers = new();
foreach (string discordid in users.Select(x => x.DiscordId).ToHashSet())
{
    latestUniqueUsers.Add(users.Where(x => x.DiscordId == discordid).OrderByDescending(x => x.Timestamp).First());
}
latestUniqueUsers = latestUniqueUsers.OrderBy(x => x.Name).ToList();
#endregion load users
#region construct image
int rowCt = (int)Math.Ceiling(latestUniqueUsers.Count / (float)Config.Current.MaxUsersPerRow);
IEnumerable<IEnumerable<User>> rows = latestUniqueUsers.OrderByDescending(x => x.Height).BreakInto(rowCt);
List<Image> rowImages = new();
string imageDescription = $"From {(rowCt > 1 ? "top to bottom, " : "")}left to right: ";
foreach(IEnumerable<User> row in rows)
{
    List<User> orderedRow = row.OrderBy(x => x.Name).ToList();
    rowImages.Add(orderedRow.Select(x => x.Image!).Merge(MergeDirection.RightLeft, 0.69f));
    string rowDescription = orderedRow.Select(x => $"{x.Name}").Aggregate((x, y) => $"{x}, {y}");
    imageDescription += $"{(rowCt > 1 ? "\n" : "")}{rowDescription}";
    TextCopy.ClipboardService.SetText(imageDescription);
}
File.WriteAllText(Path.Combine(Paths.ImageFolder, "result.txt"), imageDescription);
using Image result = ImageUtils.Merge(new Image[]
{
    Images.WatermarkToAdd,
    rowImages.Merge(MergeDirection.TopBottom, 0.42f)
}, MergeDirection.BottomTop, 0f);
result.SaveTo("result.png");
#endregion construct image