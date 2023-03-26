using grp;
using System.Net.Http;
using System.Text.Json;
Directory.CreateDirectory(Constants.ImageFolderPath);
List<string> TsvLines = File.ReadAllLines(Constants.DataPath).ToList();
Console.WriteLine($"{"discord id",-20}{"display name",-20}{"url",-128}{"height",-8}");
foreach (string tsvLine in TsvLines.Skip(1)) Console.WriteLine(tsvLine.ParseTsvLine().Readable());