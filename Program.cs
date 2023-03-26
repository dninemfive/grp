using grp;
using System.Net.Http;
using System.Text.Json;
Directory.CreateDirectory(Constants.ImageFolderPath);
List<string> TsvLines = File.ReadAllLines(Constants.DataPath).ToList();
List<(string name, int width)> Columns = new() { ("discord id", 24), ("display name", 24), ("height", 8), ("url", 128) };
IEnumerable<int> ColumnWidths = Columns.Select(x => x.width);
IEnumerable<string> underlines = Columns.Select(x => '='.Repeated(x.name.Length));
Console.WriteLine(Columns.Readable());
Console.WriteLine(underlines.Readable(ColumnWidths));
foreach (TsvLine tsvLine in TsvLines.Skip(1).Select(x => x.ParseTsvLine())) 
    Console.WriteLine(tsvLine.Columns.Readable(ColumnWidths));