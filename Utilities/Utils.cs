using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace grp
{
    public static class Utils
    {
        public static float Mean(params float[] numbers) => numbers.Aggregate((x, y) => x + y) / numbers.Length;
        public static async Task Download(string url, string? fileName = null)
        {
            fileName ??= Path.GetFileName(url);
            string targetPath = Path.Join(Constants.ImageFolderPath, fileName);
            Console.WriteLine($"Downloading {url} to {targetPath}...");
            try
            {
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
            catch (Exception e)
            {
                Console.WriteLine($"\tFailed to contact `{url}`: {e.Message}");
            }
        }
        public static string Readable<T>(params (T t, int width)[] values)
        {
            string result = "";
            foreach ((T t, int width) in values)
            {
                result += (t?.ToString() ?? "(actually null)").PadRight(width);
            }
            return result;
        }
        public static void Merge()
        {
            
        }
    }
}
