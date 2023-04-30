﻿using grp;
using System.Net.Http;
using System.Text.Json;
using System.Xml.Schema;
using System.Windows;
using System.Runtime.InteropServices;
using d9.utl;
using d9.utl.compat;

#region prepare database
Paths.CreateFolders();
if(GoogleUtils.IsValid && !GrpConfig.Current.SkipGoogleDownload) 
    GoogleUtils.Download(GrpConfig.GoogleFileId, Paths.TsvFile, "tsv".MimeType()!);
List<string> rawTsv = File.ReadAllLines(Paths.TsvFile).Skip(1).Where(x => !string.IsNullOrEmpty(x?.Trim())).ToList();
ColumnInfoSet columns = new(
    ("timestamp", 24, ColumnType.Key),
    ("discord id", 42),
    ("display name", 32, ColumnType.Nullable),
    ("url", 128),
    ("height", 8)
);
TsvDocument document = new(columns, rawTsv);
foreach (string s in document.Readable) Console.WriteLine(s);
#endregion prepare database
#region load users
List<User> users = new();
foreach (TsvRow row in document.Rows)
{
    users.Add(await User.Parse(row));
}
List<User> latestUniqueUsers = new();
foreach (string discordid in users.Select(x => x.DiscordId).ToHashSet())
{
    Console.WriteLine(users.Where(x => x.DiscordId == discordid).OrderByDescending(x => x.Timestamp).ListNotation());
    Console.WriteLine($"\t{users.Where(x => x.DiscordId == discordid).OrderByDescending(x => x.Timestamp).First()}");
    latestUniqueUsers.Add(users.Where(x => x.DiscordId == discordid).OrderByDescending(x => x.Timestamp).First());
}
latestUniqueUsers = latestUniqueUsers.OrderBy(x => x.Name).ToList();
foreach (User user in latestUniqueUsers.OrderByDescending(x => x.Height)) Console.WriteLine(user);
#endregion load users
#region construct image
// float medianExcessAlpha = latestUniqueUsers.Select(x => (float)x.ExcessAlpha).Median((x, y) => MiscUtils.Mean(x, y));
long maxNormalExcessAlpha = 2137666; // determined by inspection
int rowCt = (int)Math.Ceiling(latestUniqueUsers.Count / (float)GrpConfig.Current.MaxUsersPerRow);
IEnumerable<IEnumerable<User>> rows = latestUniqueUsers
                                        .OrderByDescending(x => MathF.Max(0, x.ExcessAlpha - maxNormalExcessAlpha))
                                        .ThenByDescending(x => x.Height)
                                        .BreakInto(rowCt);
List<Image> rowImages = new();
string imageDescription = $"From {(rowCt > 1 ? "top to bottom, " : "")}left to right: ";
foreach(IEnumerable<User> row in rows)
{
    List<User> orderedRow = row.OrderBy(x => x.Name).ToList();
    rowImages.Add(orderedRow.Select(x => x.Image!).Merge(MergeDirection.RightLeft, 0.80f));
    string rowDescription = orderedRow.Select(x => $"{x.Name}").Aggregate((x, y) => $"{x}, {y}");
    imageDescription += $"{(rowCt > 1 ? "\n" : "")}{rowDescription}";    
}
if (GrpConfig.Current.SaveDescToFile) File.WriteAllText(Path.Combine(Paths.ImageFolder, "result.txt"), imageDescription);
if (GrpConfig.Current.CopyDescToClipboard) TextCopy.ClipboardService.SetText(imageDescription);
using Image result = ImageUtils.Merge(new Image[]
{
    Images.WatermarkToAdd,
    rowImages.Merge(MergeDirection.TopBottom, 0.42f)
}, MergeDirection.BottomTop, 0f);
result.SaveTo("result.png");
#endregion construct image