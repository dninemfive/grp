using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    public static class NetUtils
    {
        public static readonly HttpClient Client = new();
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
            Console.Write($"Downloading {url} to {targetPath}... ");
            try
            {
                using HttpResponseMessage response = await Client.GetAsync(url);
                Console.Write($"\t{(response.IsSuccessStatusCode ? "✔️" : "❌")}\t{(int)response.StatusCode} {response.ReasonPhrase}");
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        using Stream stream = await response.Content.ReadAsStreamAsync();
                        using FileStream fs = new(targetPath, FileMode.Create);
                        await stream.CopyToAsync(fs);
                        Console.WriteLine(" ...Downloaded.");
                        return targetPath;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($" ...Download failed: {e.Message}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($" ...Failed to contact `{url}`: {e.Message}");
            }
            return null;
        }
        /// <summary>
        /// Attempts to download a file to the <see cref="Constants.ImageFolderPath">default image folder</see> and prints whether or not it was successful, 
        /// as well as the response code.
        /// </summary>
        /// <param name="url">The URL of the file to download.</param>
        /// <param name="filename">The name the file should have when downloaded.</param>
        /// <returns>The <see cref="Image"/> downloaded, if successful, or <see langword="null"/> otherwise.</returns>
        public static async Task<Image?> DownloadImage(string url, string filename)
        {
            string? resultPath = await Download(url, filename);
            if (resultPath is null) return null;
            return IoUtils.LoadImage(resultPath);
        }
    }
}
