using grp;
using System.Net.Http;
using System.Text.Json;
Directory.CreateDirectory(Constants.ImageFolderPath);
List<string> rawTsv = File.ReadAllLines(Constants.DataPath).ToList();