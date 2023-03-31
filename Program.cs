using grp;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
Directory.CreateDirectory(Paths.ImageFolder);
List<string> rawTsv = File.ReadAllLines(Paths.DataFile).Skip(1).ToList();
ColumnInfoSet columns = new(
    ("timestamp", 24, ColumnType.Key),
    ("discord id", 42),
    ("display name", 32, ColumnType.Nullable),
    ("url", 128),
    ("height", 8)
);
TsvDocument document = new(columns, rawTsv);
foreach (string s in document.Readable) Console.WriteLine(s);
List<User> Users = new();
foreach (TsvRow row in document.Rows)
{
    Users.Add(await User.Parse(row));
}
foreach (User user in Users) Console.WriteLine($"{user} : {user.Image?.Height.PrintNullable()}");
using Image result = Users.Select(x => x.Image!).Merge();
result.SaveTo("result.png");
using Image<Rgba32> watermarkMask = Constants.WatermarkForSubtraction.CloneAs<Rgba32>();
HashSet<(int x, int y)> points = new();
Rgba32 transparent = Color.Transparent;
watermarkMask.ProcessPixelRows(accessor =>
{
    for(int y = 0; y < accessor.Height; y++)
    {
        Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
        for(int x = 0; x < pixelRow.Length; x++)
        {
            points.Add((x, y));
        }
    }
});