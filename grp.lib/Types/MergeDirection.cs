namespace d9.grp.lib;

/// <summary>
/// The direction in which to <see cref="ImageUtils.Merge(IEnumerable{Image}, MergeDirection,
/// float)">merge</see> an image. <br/><inheritdoc cref="ImageUtils.Merge(IEnumerable{Image},
/// MergeDirection, float)" path="/param[@name='direction']/list"/>
/// </summary>
public enum MergeDirection
{
    LeftRight,
    RightLeft,
    TopBottom,
    BottomTop
}