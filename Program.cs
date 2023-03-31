using grp;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
Directory.CreateDirectory(Paths.ImageFolder);
List<string> rawTsv = File.ReadAllLines(Paths.DataFile).Skip(1).ToList();
ColumnInfoSet columns = new(
    ("timestamp", 24, ColumnType.Key),
    ("discord id", 42),
    ("display name", 32, ColumnType.Nullable),
    ("url", 128),
    ("height", 8)
);
TsvDocument document = new(columns, rawTsv);
foreach (string s in document.Readable) Console.WriteLine(s);
List<User> Users = new();
foreach (TsvRow row in document.Rows)
{
    Users.Add(await User.Parse(row));
}
foreach (User user in Users) Console.WriteLine($"{user} : {user.Image?.Height.PrintNullable()}");
using Image result = Users.Select(x => x.Image!).Merge();
result.SaveTo("result.png");