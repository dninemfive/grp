using System.Net.Http;
using System.Text.Json;
const string basePath = @"C:\Users\dninemfive\Documents\workspaces\misc\grp";
HttpClient client = new();
async Task Download(string url, string targetPath)
{
    Console.WriteLine($"Downloading {url} to {targetPath}...");
    using HttpResponseMessage response = await client.GetAsync(url);
    if(response.IsSuccessStatusCode)
    {
        using Stream stream = await response.Content.ReadAsStreamAsync();
        using FileStream fs = new(targetPath, FileMode.Create);
        await stream.CopyToAsync(fs);
        Console.WriteLine($"\t✔️\t{response.StatusCode} {response.ReasonPhrase}");
    }
    else
    {
        Console.WriteLine($"\t❌\t{response.StatusCode} {response.ReasonPhrase}");
    }
}
await Download("https://cdn.discordapp.com/attachments/771462457436667935/1089635920661196860/Untitled.jpg", @$"{basePath}\test.png");