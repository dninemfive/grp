using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    /// <summary>
    /// The type a column of a CSV table has.
    /// <list type="bullet">
    /// <item><c>Key</c> columns must have a value, and are used to index the table. There must be precisely one <c>Key</c> column in any given table.</item>
    /// <item><c>Normal</c> columns must have a value, but are not special otherwise.</item>
    /// <item><c>Nullable</c> columns may have no value in a given row, which is represented as a <c>null</c> value in code.</item>
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
        /// Columns of this type may have no value in a given row, which is represented as a <c>null</c> value in code.
        /// </summary>
        Nullable
    }
    public enum AutocropType { Horizontal, Vertical, Both }
    public enum MergeDirection { LeftRight, RightLeft, TopBottom, BottomTop }
}
