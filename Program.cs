using grp;
using System.Net.Http;
using System.Text.Json;
Directory.CreateDirectory(Constants.ImageFolderPath);
List<string> rawTsv = File.ReadAllLines(Constants.DataPath).ToList();
ColumnInfoSet columns = new(
    ("timestamp", 24, ColumnType.Key),
    ("discord id", 42),
    ("display name", 32, ColumnType.Nullable),
    ("height", 8),
    ("url", 128)
    );
TsvDocument document = new(columns, rawTsv);