using d9.utl;

namespace d9.grp.lib;
public class GroupPhotoGenerator
{
    // determined by inspection
    // todo: figure out how this was calculated and dynamically calculate it instead
    private const long _maxNormalAlpha = 2137666;
    public bool SaveDescToFile = true;
    public bool CopyDescToClipboard = true;
    public int MaxUsersPerRow;
    public IEnumerable<IEnumerable<UserImage>> Sort(IEnumerable<UserImage> users)
    => users.OrderByDescending(x => MathF.Max(0, x.ExcessAlpha - _maxNormalAlpha))
            .ThenByDescending(x => x.User.Height)
            .ThenBy(x => x.User.Name)
            .Chunk(MaxUsersPerRow);
    public Image ConstructImage(IEnumerable<UserImage> users, out string description)
        => ConstructImage(Sort(users), out description);
    // todo: description should be generated separately
    public Image ConstructImage(IEnumerable<IEnumerable<UserImage>> rows, out string description)
    {
        List<Image> rowImages = new();
        int rowCt = rows.Count();
        description = $"From {(rowCt > 1 ? "top to bottom, " : "")}left to right: ";
        foreach (IEnumerable<UserImage> row in rows)
        {
            rowImages.Add(row.Select(x => x.Image!).Merge(MergeDirection.RightLeft, 0.80f));
            string rowDescription = row.Select(x => $"{x.User.Name}").JoinWithDelimiter(", ");
            description += $"{(rowCt > 1 ? "\n" : "")}{rowDescription}";
        }
        using Image result = ImageUtils.Merge(
        [
            Images.WatermarkToAdd,
            rowImages.Merge(MergeDirection.TopBottom, 0.42f)
        ], MergeDirection.BottomTop, 0f);
        return result;
    }
}