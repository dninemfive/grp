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
    await Utils.Download(row["url"]!, $"{row["discord id"]!}{Path.GetExtension(row["url"]!)}");
}