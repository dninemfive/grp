using d9.utl;
using d9.utl.compat;
using System.Text.Json;

namespace d9.grp;
public static class Program
{
    public static async Task Main()
    {
        Paths.CreateFolders();
        if (GoogleUtils.HasValidAuthConfig && !GrpConfig.Current.SkipGoogleDownload)
            GoogleUtils.Download(GrpConfig.GoogleFileId, Paths.TsvFile, "tsv".MimeType()!);
        ConstructImage(await LoadUsersFrom(DocumentAt(Paths.TsvFile)));
    }
    private static readonly ColumnInfoSet _columns = new(
            ("timestamp", 24, ColumnType.Key),
            ("discord id", 42),
            ("display name", 32, ColumnType.Nullable),
            ("url", 128),
            ("height", 8)
        );
    private static TsvDocument DocumentAt(string path)
    {
        List<string> rawTsv = File.ReadAllLines(path).Skip(1).Where(x => !string.IsNullOrEmpty(x?.Trim())).ToList();
        TsvDocument document = new(_columns, rawTsv);
        foreach (string s in document.Readable)
            Console.WriteLine(s);
        return document;
    }
    private static async Task<IEnumerable<IEnumerable<User>>> LoadUsersFrom(TsvDocument document)
    {
        List<User> users = new();
        foreach (TsvRow row in document.Rows)
            users.Add(await User.Parse(row));
        List<User> latestUniqueUsers = new();
        foreach (string discordid in users.Select(x => x.DiscordId).ToHashSet())
        {
            Console.WriteLine(users.Where(x => x.DiscordId == discordid).OrderByDescending(x => x.Timestamp).ListNotation());
            Console.WriteLine($"\t{users.Where(x => x.DiscordId == discordid).OrderByDescending(x => x.Timestamp).First()}");
            latestUniqueUsers.Add(users.Where(x => x.DiscordId == discordid).OrderByDescending(x => x.Timestamp).First());
        }
        latestUniqueUsers = latestUniqueUsers.OrderBy(x => x.Name).ToList();
        foreach (User user in latestUniqueUsers.OrderByDescending(x => x.Height))
            Console.WriteLine(user);
        return latestUniqueUsers.OrderByDescending(x => MathF.Max(0, x.ExcessAlpha - _maxNormalAlpha))
                                .ThenByDescending(x => x.Height)
                                .Chunk(GrpConfig.Current.MaxUsersPerRow);
    }
    
}