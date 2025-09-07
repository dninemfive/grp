namespace d9.grp.lib;
public class GroupPhotoGenerator
{
    // determined by inspection
    // todo: figure out how this was calculated and dynamically calculate it instead
    private const long _maxNormalAlpha = 2137666;
    public bool SaveDescToFile = true;
    public bool CopyDescToClipboard = true;
    public async Task<IEnumerable<IEnumerable<User>>>
    public Image ConstructImage(IEnumerable<IEnumerable<User>> rows, out string description)
    {
        List<Image> rowImages = new();
        int rowCt = rows.Count();
        description = $"From {(rowCt > 1 ? "top to bottom, " : "")}left to right: ";
        foreach (IEnumerable<User> row in rows)
        {
            List<User> orderedRow = row.OrderBy(x => x.Name).ToList();
            rowImages.Add(orderedRow.Select(x => x.Image!).Merge(MergeDirection.RightLeft, 0.80f));
            string rowDescription = orderedRow.Select(x => $"{x.Name}").Aggregate((x, y) => $"{x}, {y}");
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