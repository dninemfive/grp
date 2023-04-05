using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using System.Text.Json;
using Google.Apis.Services;
using Google.Apis.Download;

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
        /// <summary>
        /// Gets the Google Auth certificate from the (privately-stored) key and password files.
        /// </summary>
        /// <remarks>Largely a copy of code from <see href="https://www.daimto.com/google-drive-authentication-c/">this example</see>.<br/>
        /// <br/> Apparently the password is always <c>notasecret</c> and that can't be changed, which is strange.</remarks>
        private static X509Certificate2 Certificate
        {
            get
            {
                if (Config.Current.GoogleAuth is null) throw new Exception("Attempted to get Google account certificate, but no auth config was found!");
                return new(Paths.GoogleKey, "notasecret", X509KeyStorageFlags.Exportable);
            }
        }
        /// <summary>
        /// Constructs a ServiceAccountCredential initializer from the <see cref="Certificate"/>.
        /// </summary>
        /// <remarks>Largely a copy of code from <see href="https://www.daimto.com/google-drive-authentication-c/">this example</see>.</remarks>
        private static ServiceAccountCredential.Initializer CredentialInitializer
        {
            get
            {
                if (Config.Current.GoogleAuth is null) throw new Exception("Attempted to get Google account credentials, but no auth config was found!");
                return new ServiceAccountCredential.Initializer(Config.Current.GoogleAuth.Email) { Scopes = new[] { DriveService.Scope.Drive } }
                    .FromCertificate(Certificate);
            }
        }
        /// <summary>
        /// Constructs a credential using the <see cref="CredentialInitializer"/>.
        /// </summary>
        /// <remarks>Largely a copy of code from <see href="https://www.daimto.com/google-drive-authentication-c/">this example</see>.</remarks>
        public static ServiceAccountCredential Credential => new(CredentialInitializer);
        /// <summary>
        /// Gets the Drive service using the <see cref="Credential"/> previously established.
        /// </summary>
        public static DriveService DriveService => new(new BaseClientService.Initializer() 
        {
            HttpClientInitializer = Credential,
            ApplicationName = "misc-grp"
        });
        /// <summary>
        /// Attempts to download the data TSV file from a Drive URL to the <see cref="Paths.BaseFolder">default base folder</see> 
        /// and prints whether or not it was successful, as well as the response code.
        /// </summary>
        /// <remarks>The file must be shared, through the Sheets UI, with the <see cref="GoogleAuthConfig.Email">email associated with the service account</see>.</remarks>
        /// <param name="fileId">The <see cref="GoogleAuthConfig.FileId">Sheets ID</see> of the file to download.</param>
        /// <param name="fileName">The name the file should have when downloaded.</param>
        /// <returns>The path to the downloaded file, if successfully downloaded, or <see langword="null"/> otherwise.</returns>
        public static string? DownloadTsv(string fileId, string filename)
        {
            FilesResource.ExportRequest request = new(DriveService, fileId, "text/tab-separated-values");
            using FileStream fs = new(filename.AbsoluteOrInBaseFolder(), FileMode.Create);
            IDownloadProgress progress = request.DownloadWithStatus(fs);
            try
            {
                progress.ThrowOnFailure();
                return filename.AbsoluteOrInBaseFolder();
            } catch(Exception e)
            {
                Console.WriteLine($"Error when downloading file from Drive!\n\t{e.Message}");
                return null;
            }
        }
    }
}
