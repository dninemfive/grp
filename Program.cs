using grp;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
Directory.CreateDirectory(Paths.ImageFolder);
List<string> rawTsv = File.ReadAllLines(Paths.DataFile).Skip(3).ToList();
ColumnInfoSet columns = new(
    ("timestamp", 24, ColumnType.Key),
    ("discord id", 42),
    ("display name", 32, ColumnType.Nullable),
    ("url", 128),
    ("height", 8)
);
TsvDocument document = new(columns, rawTsv);
foreach (string s in document.Readable) Console.WriteLine(s);
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
latestUniqueUsers = latestUniqueUsers.OrderBy(x => x.Height.InCentimeters).ToList();
foreach (User user in latestUniqueUsers) Console.WriteLine($"{user}");
foreach (User user in latestUniqueUsers) user.Image!.SaveTo($"debug/{user.DiscordId}.png");
using Image result = latestUniqueUsers.Select(x => x.Image!).Merge(0.5f);
result.SaveTo("result.png");