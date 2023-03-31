using grp;
using System.Net.Http;
using System.Text.Json;
Directory.CreateDirectory(Constants.ImageFolderPath);
List<string> rawTsv = File.ReadAllLines(Constants.DataPath).Skip(1).ToList();
ColumnInfoSet columns = new(
    ("timestamp", 24, ColumnType.Key),
    ("discord id", 42),
    ("display name", 32, ColumnType.Nullable),
    ("url", 128),
    ("height", 8)
);
TsvDocument document = new(columns, rawTsv);
foreach (string s in document.Readable) Console.WriteLine(s);
foreach (TsvRow row in document.Rows)
{
    // await Utils.Download(row["url"]!, $"{row["discord id"]!}{Path.GetExtension(row["url"]!)}");
}
using Image img = Utils.LoadImage("test.png"), img2 = Utils.LoadImage("test.png");
using Image result = new Image<Rgba32>(img.Width + img2.Width, img.Height);
img.Mutate((context) => context.Flip(FlipMode.Horizontal));
result.Mutate((context) => context.DrawImage(img, 1));
result.Mutate((context) => context.DrawImage(img2, new Point(img.Width, 0), 1));
result.SaveTo("test2.png");
/* img.Mutate((context) => context.Resize(new ResizeOptions()
{
    Position = AnchorPositionMode.Left,
    Size = new(img.Width * 2, img.Height)
})); */