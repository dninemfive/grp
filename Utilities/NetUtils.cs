using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;

namespace grp
{
    public static class NetUtils
    {
        /// <summary>
        /// The <see href="https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-8.0">HttpClient</see> used by this application.
        /// </summary>
        /// <remarks>According to the <see href="https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-8.0#instancing">documentation</see>,
        /// only one <see cref="HttpClient"/> is meant to be used per application.</remarks>
        public static readonly HttpClient Client = new();
        /// <summary>
        /// Attempts to download a file to the <see cref="Images.ImageFolderPath">default image folder</see> and prints whether or not it was successful, 
        /// as well as the response code.
        /// </summary>
        /// <param name="url">The URL of the file to download.</param>
        /// <param name="fileName">The name the file should have when downloaded. If not specified, defaults to the name of the file specified in the <c>url</c>.</param>
        /// <returns>The path to the downloaded file, if successfully downloaded, or <see langword="null"/> otherwise.</returns>
        public static async Task<string?> Download(string url, string? fileName = null)
        {
            fileName ??= Path.GetFileName(url);
            string targetPath = Path.Join(Paths.ImageFolder, fileName);
            //Console.Write($"Downloading {url} to {targetPath}... ");
            Console.Write($"{fileName,-32}");
            try
            {
                using HttpResponseMessage response = await Client.GetAsync(url);
                Console.WriteLine($"\t{(response.IsSuccessStatusCode ? "✔️" : "❌")}\t{(int)response.StatusCode} {response.ReasonPhrase}");
                long? contentLength = response.Content.Headers.ContentLength;
                if (contentLength > 1e6) throw new Exception($"File at {url} was of size {contentLength}, which is implausibly large!");
                string? mediaType = response.Content.Headers.ContentType?.MediaType;
                if (mediaType != "image/png") throw new Exception($"File at {url} was of type {mediaType}, not image/png!");
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        using Stream stream = await response.Content.ReadAsStreamAsync();
                        using FileStream fs = new(targetPath, FileMode.Create);
                        await stream.CopyToAsync(fs);
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
                Console.WriteLine($"\t...Failed to contact `{url}`: {e.Message}");
            }
            return null;
        }
        /// <summary>
        /// Attempts to download a file to the <see cref="Images.ImageFolderPath">default image folder</see> and prints whether or not it was successful, 
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
