using System.Net.Http;
using System.Text.Json;
const string picrewUrl = "https://picrew.me";
HttpClient client = new();
void Print(JsonProperty prop, int indent = 0)
{
    string indents = "";
    for (int i = 0; i < indent; i++) indents += "  ";
    if (prop.Name == "url") Console.WriteLine($"{indents}https://picrew.me{prop.Value}");
    else
    {
        Console.WriteLine($"{indents}{prop.Name}");
        foreach(JsonProperty child in prop.Value.EnumerateObject())
        {
            Print(child, indent + 1);
        }
    }
}
async Task Acquire(JsonProperty prop, string? previousPath = null, int indent = 0)
{
    previousPath ??= "C:/Users/dninemfive/Documents/workspaces/misc/grp/files/";
    string indents = "";
    for (int i = 0; i < indent; i++) indents += "  ";
    if(prop.Name == "url")
    {
        string fileName = Path.GetFileName(prop.Value.ToString());
        string fullPath = Path.Join(previousPath, fileName);
        if(File.Exists(fullPath))
        {
            Console.WriteLine($"{indents}{fileName} already exists.");
            return;
        }
        await Download(prop.Value.ToString(), fullPath, indents);
    } 
    else
    {
        Console.WriteLine($"{indents}{prop.Name}");
        string fullPath = Path.Join(previousPath, prop.Name);
        Directory.CreateDirectory(fullPath);
        foreach (JsonProperty child in prop.Value.EnumerateObject()) await Acquire(child, fullPath, indent + 1);
    }    
}
async Task Download(string urlPath, string targetPath, string indents = "")
{
    string url = $"{picrewUrl}{urlPath}";
    Console.WriteLine($"{indents}{Path.GetFileName(targetPath)} does not exist. Downloading from {url}...");
    using HttpResponseMessage response = await client.GetAsync(url);
    if(response.IsSuccessStatusCode)
    {
        using Stream stream = await response.Content.ReadAsStreamAsync();
        using FileStream fs = new(targetPath, FileMode.Create);
        await stream.CopyToAsync(fs);
        Console.WriteLine($"{indents}  Successfully saved to {targetPath}");
    }
    else
    {
        Console.WriteLine($"{indents}  Download failed: {response.StatusCode} {response.ReasonPhrase}");
    }
}
JsonDocument doc = JsonDocument.Parse(File.OpenRead("C:/Users/dninemfive/Documents/picrew files.json"));
foreach (JsonProperty prop in doc.RootElement.EnumerateObject())
{
    await Acquire(prop);
}