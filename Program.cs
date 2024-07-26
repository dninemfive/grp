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
    // determined by inspection
    private const long _maxNormalAlpha = 2137666;
    private static async Task<IEnumerable<IEnumerable<User>>> LoadUsersFrom(TsvDocument document)
    {
        List<User> users = new();
        foreach (TsvRow row in document.Rows)
        {
            users.Add(await User.Parse(row));
        }
        List<User> latestUniqueUsers = new();
        foreach (string discordid in users.Select(x => x.DiscordId).ToHashSet())
        {
            Console.WriteLine(users.Where(x => x.DiscordId == discordid).OrderByDescending(x => x.Timestamp).ListNotation());
            Console.WriteLine($"\t{users.Where(x => x.DiscordId == discordid).OrderByDescending(x => x.Timestamp).First()}");
            latestUniqueUsers.Add(users.Where(x => x.DiscordId == discordid).OrderByDescending(x => x.Timestamp).First());
        }
        latestUniqueUsers = latestUniqueUsers.OrderBy(x => x.Name).ToList();
        foreach (User user in latestUniqueUsers.OrderByDescending(x => x.Height))
        {
            Console.WriteLine(user);
        }
        int rowCt = (int)Math.Ceiling(latestUniqueUsers.Count / (float)GrpConfig.Current.MaxUsersPerRow);
        return latestUniqueUsers.OrderByDescending(x => MathF.Max(0, x.ExcessAlpha - _maxNormalAlpha))
                                .ThenByDescending(x => x.Height)
                                .Chunk(rowCt);
    }
    private static void ConstructImage(IEnumerable<IEnumerable<User>> rows)
    {
        List<Image> rowImages = new();
        int rowCt = rows.Count();
        string imageDescription = $"From {(rowCt > 1 ? "top to bottom, " : "")}left to right: ";
        foreach (IEnumerable<User> row in rows)
        {
            List<User> orderedRow = row.OrderBy(x => x.Name).ToList();
            rowImages.Add(orderedRow.Select(x => x.Image!).Merge(MergeDirection.RightLeft, 0.80f));
            string rowDescription = orderedRow.Select(x => $"{x.Name}").Aggregate((x, y) => $"{x}, {y}");
            imageDescription += $"{(rowCt > 1 ? "\n" : "")}{rowDescription}";
        }
        if (GrpConfig.Current.SaveDescToFile)
            File.WriteAllText(Path.Combine(Paths.ImageFolder, "result.txt"), imageDescription);
        if (GrpConfig.Current.CopyDescToClipboard)
            TextCopy.ClipboardService.SetText(imageDescription);
        using Image result = ImageUtils.Merge(new Image[]
        {
            Images.WatermarkToAdd,
            rowImages.Merge(MergeDirection.TopBottom, 0.42f)
        }, MergeDirection.BottomTop, 0f);
        result.SaveTo("result.png");
    }
}