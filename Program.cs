using grp;
using System.Net.Http;
using System.Text.Json;
Directory.CreateDirectory(Constants.ImageFolderPath);
List<string> rawTsv = File.ReadAllLines(Constants.DataPath).ToList();
TsvDocument document = new(new(new("timestamp", 24, key: true), ("discord id", 32), new("name", ), rawTsv));