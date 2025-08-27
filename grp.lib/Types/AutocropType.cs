namespace d9.grp.lib;

/// <summary>
/// What kind of <see cref="ImageUtils.Autocrop(Image, AutocropType)">autocrop</see> to perform.
/// <list type="bullet">
/// <item><c>Horizontal</c>: <inheritdoc cref="Horizontal" path="/summary"/></item>
/// <item><c>Vertical</c>: <inheritdoc cref="Vertical" path="/summary"/></item>
/// <item><c>Both</c>: <inheritdoc cref="Both" path="/summary"/></item>
/// </list>
/// </summary>
public enum AutocropType
{
    /// <summary>
    /// Crop only the left and right side of the image.
    /// </summary>
    Horizontal,
    /// <summary>
    /// Crop only the top and bottom of the image.
    /// </summary>
    Vertical,
    /// <summary>
    /// Crop both the left and right and top and bottom of the image.
    /// </summary>
    Both
}