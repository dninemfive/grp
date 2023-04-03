using grp;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Xml.Schema;

const int maxUsersPerRow = 12;
#region prepare database
Directory.CreateDirectory(Paths.ImageFolder);
NetUtils.DownloadTsv(File.ReadAllText(Paths.FileId), Paths.DataFile);
List<string> rawTsv = File.ReadAllLines(Paths.DataFile).Skip(1).Where(x => !string.IsNullOrEmpty(x?.Trim())).ToList();
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
// foreach (User user in latestUniqueUsers) user.Image!.SaveTo($"debug/{user.DiscordId}.png");
#endregion load users
#region construct image
int rowCt = (int)Math.Ceiling(latestUniqueUsers.Count() / (float)maxUsersPerRow);
IEnumerable<IEnumerable<User>> rows = latestUniqueUsers.OrderByDescending(x => x.Height).BreakInto(rowCt);
List<Image> rowImages = new();
string imageDescription = $"From {(rowCt > 1 ? "top to bottom, " : "")}left to right: ";
foreach(IEnumerable<User> row in rows)
{
    List<User> orderedRow = row.OrderBy(x => x.Name).ToList();
    rowImages.Add(orderedRow.Select(x => x.Image!).Merge(MergeDirection.RightLeft, 0.69f));
    string rowDescription = orderedRow.Select(x => $"{x.Name}").Aggregate((x, y) => $"{x}, {y}");
    // Console.WriteLine(rowDescription);
    imageDescription += $"{(rowCt > 1 ? "\n" : "")}{rowDescription}";
}
File.WriteAllText(Path.Combine(Paths.ImageFolder, "result.txt"), imageDescription);
using Image result = ImageUtils.Merge(new Image[]
{
    Images.WatermarkToAdd,
    rowImages.Merge(MergeDirection.TopBottom, 0.42f)
}, MergeDirection.BottomTop, 0f);
result.SaveTo("result.png");
#endregion construct image