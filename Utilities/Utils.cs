using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    /// <summary>
    /// Non-extension utility methods.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Gets the mean of an arbitrary set of <see langword="float"/>s.
        /// </summary>
        /// <param name="numbers">An array of <see langword="float"/>s to be averaged.</param>
        /// <returns>The <see href="https://en.wikipedia.org/wiki/Arithmetic_mean">arithmetic mean</see> of the given <c>numbers</c>.</returns>
        public static float Mean(params float[] numbers) => numbers.Aggregate((x, y) => x + y) / numbers.Length;
        /// <summary>
        /// Attempts to download a file to the <see cref="Constants.ImageFolderPath">default image folder</see> and prints whether or not it was successful, 
        /// as well as the response code.
        /// </summary>
        /// <param name="url">The URL of the file to download.</param>
        /// <param name="fileName">The name the file should have when downloaded. If not specified, defaults to the name of the file specified in the <c>url</c>.</param>
        /// <returns>The path to the downloaded file, if successfully downloaded, or <see langword="null"/> otherwise.</returns>
        public static async Task<string?> Download(string url, string? fileName = null)
        {
            fileName ??= Path.GetFileName(url);
            string targetPath = Path.Join(Paths.ImageFolder, fileName);
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
                        return targetPath;
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
            return null;
        }
        public static async Task<Image?> DownloadImage(string url, string filename)
        {
            string? resultPath = await Download(url, filename);
            if (resultPath is null) return null;
            return LoadImage(resultPath);
        }
        /// <summary></summary>
        /// <typeparam name="T">The type of the objects to print.</typeparam>
        /// <param name="values">An array holding the objects to print.</param>
        /// <returns>A string corresponding to the objects <c>t</c> in order, with columns padded to <c>width</c>.</returns>
        public static string InColumns<T>(params (T t, int width)[] values)
        {
            string result = "";
            foreach ((T t, int width) in values)
            {
                result += t.PrintNullable().PadRight(width);
            }
            return result;
        }
        public static Image LoadImage(string path)
        {
            if (!Path.IsPathFullyQualified(path)) path = path.InImageFolder(); 
            using FileStream fs = File.OpenRead(path);
            return Image.Load(fs);
        }
        public static IEnumerable<Image> LoadImages(params string[] paths)
        {
            foreach (string path in paths) yield return LoadImage(path);
        }
        public static Image Merge(params Image[] images)
        {
            int width = images.Select(x => x.Width).Sum();
            int height = images.Select(x => x.Height).Max();
            Image result = new Image<Rgba32>(width, height);
            int currentLeftSide = 0;
            foreach(Image img in images)
            {
                result.Mutate((context) => context.DrawImage(img, new Point(currentLeftSide, 0), 1));
                currentLeftSide += (int)(img.Width * 1f);
            }
            return result;
        }
    }
}
