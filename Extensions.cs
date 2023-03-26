using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    public static class Constants
    {
        public const string BASE_PATH = "C:/Users/dninemfive/Documents/workspaces/misc/grp";
        public static readonly string ImageFolderPath = Path.Join(BASE_PATH, "images");
        public static readonly string DataPath = Path.Join(BASE_PATH, "grp responses.tsv");
        public static readonly HttpClient Client = new();
    }
    public static class Extensions
    {
        public static (T, U) SplitAndParse<T, U>(this string s, string delimiter, Func<string, T> leftParser, Func<string, U> rightParser)
        {
            string[] split = s.Split(delimiter);
            if (split.Length != 2) throw new Exception($"String `{s}` should be split by delimiter `{delimiter}` into exactly two parts, " +
                                                       $"but it was split into {split.Length} parts instead.");
            return (leftParser(split[0]), rightParser(split[1]));
        }
        public static (T, T) SplitAndParse<T>(this string s, string delimiter, Func<string, T> parser) => s.SplitAndParse(delimiter, parser, parser);
        private const int EXPECTED_TSV_LENGTH = 5;
        public static (string id, string? name, string height, string weight) ParseTsvLine(this string s)
        {
            string[] split = s.Split("\t");
            if (split.Length != EXPECTED_TSV_LENGTH) throw new Exception($"TSV lines should have precisely {EXPECTED_TSV_LENGTH} elements, but `{s}` has {s.Length} elements.");
            string? name = string.IsNullOrEmpty(split[2]) ? null : split[2];
            return (split[1], name, split[3], split[4]);
        }
        public static string Readable(this IEnumerable<(string s, int width)> enumerable) => Utils.Readable(enumerable.ToArray());
        public static string Readable(this IEnumerable<string> strings, IEnumerable<int> widths) => Readable(strings.Zip(widths));
        public static string Repeated(this char c, int times)
        {
            string result = "";
            for (int i = 0; i < times; i++) result += c;
            return result;
        }
    }
    public static class Utils
    {
        public static float Mean(params float[] numbers) => numbers.Aggregate((x, y) => x + y) / numbers.Length;
        public static async Task Download(string url, string? targetPath = null)
        {
            targetPath ??= Path.Join(Constants.ImageFolderPath, Path.GetFileName(url));
            Console.WriteLine($"Downloading {url} to {targetPath}...");
            using HttpResponseMessage response = await Constants.Client.GetAsync(url);
            Console.WriteLine($"\t{(response.IsSuccessStatusCode ? "✔️" : "❌")}\t{(int)response.StatusCode} {response.ReasonPhrase}");
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    using Stream stream = await response.Content.ReadAsStreamAsync();
                    using FileStream fs = new(targetPath, FileMode.Create);
                    await stream.CopyToAsync(fs);
                    Console.WriteLine("\tDownloaded.");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"\tDownload failed: {e.Message}");
                }
            }
        }
        public static string Readable(params (string s, int width)[] values)
        {
            string result = "";
            foreach ((string s, int width) in values)
            {
                result += s.PadRight(width);
            }
            return result;
        }        
    }
}
