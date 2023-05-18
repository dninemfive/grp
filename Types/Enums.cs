using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.grp
{
    /// <summary>
    /// The type a column of a CSV table has.
    /// <list type="bullet">
    /// <item><c>Key</c>: <inheritdoc cref="Key" path="/summary"/></item>
    /// <item><c>Normal</c>: <inheritdoc cref="Normal" path="/summary"/></item>
    /// <item><c>Nullable</c>: <inheritdoc cref="Nullable" path="/summary"/></item>
    /// </list>
    /// </summary>
    public enum ColumnType {
        /// <summary>
        /// Columns of this type must have a value, and are used to index the table. There must be precisely one <c>Key</c> column in any given table.
        /// </summary>
        Key,
        /// <summary>
        /// Columns of this type must have a value, but are not special otherwise.
        /// </summary>
        Normal,
        /// <summary>
        /// Columns of this type may have no value in a given row, which is represented as a <see langword="null"/> value in code.
        /// </summary>
        Nullable
    }
    /// <summary>
    /// What kind of <see cref="ImageUtils.Autocrop(Image, AutocropType)">autocrop</see> to perform.
    /// <list type="bullet">
    /// <item><c>Horizontal</c>: <inheritdoc cref="Horizontal" path="/summary"/></item>
    /// <item><c>Vertical</c>: <inheritdoc cref="Vertical" path="/summary"/></item>
    /// <item><c>Both</c>: <inheritdoc cref="Both" path="/summary"/></item>
    /// </list>
    /// </summary>
    public enum AutocropType {
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
    /// <summary>
    /// The direction in which to <see cref="ImageUtils.Merge(IEnumerable{Image}, MergeDirection, float)">merge</see> an image. <br/>
    /// <inheritdoc cref="ImageUtils.Merge(IEnumerable{Image}, MergeDirection, float)" path="/param[@name='direction']/list"/>
    /// </summary>
    public enum MergeDirection {
        LeftRight, 
        RightLeft, 
        TopBottom, 
        BottomTop 
    }
}
