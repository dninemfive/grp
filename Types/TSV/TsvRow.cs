using d9.utl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.grp
{
    /// <summary>
    /// Represents a single row in a <see cref="TsvDocument"/>.
    /// </summary>
    public class TsvRow
    {
        /// <summary>
        /// Holds the value of each column indexed by the column's name.
        /// </summary>
        private readonly Dictionary<string, string?> columnValues = new();
        /// <summary>
        /// The name of this row's <see cref="ColumnType.Key">Key</see> column.
        /// </summary>
        /// <remarks>Should never have the default value <c>no key found</c> because of validation in the constructor.
        /// A default value is specified only so the compiler doesn't complain about nullability.</remarks>
        public string Key { get; } = "no key found";
        /// <summary>
        /// The <see cref="ColumnInfoSet"/> describing this row's columns.
        /// </summary>
        public ColumnInfoSet Columns { get; }
        /// <summary>
        /// Creates a new <c>TsvRow</c> with the specified columns and values. Ensures that each column meets the requirements of its <see cref="ColumnType"/>,
        /// namely:<br/><inheritdoc cref="ColumnType" path="/summary/list"/>
        /// </summary>
        /// <param name="columns"><inheritdoc cref="Columns" path="/summary"/></param>
        /// <param name="entries">A set of pairs of column names and the values those respective columns should have.</param>
        /// <exception cref="Exception">Thrown if the columns do not meet the above-specified requirements.</exception>
        public TsvRow(ColumnInfoSet columns, params (string k, string v)[] entries)
        {
            IEnumerable<string> nonNullableColumnNames = columns.Where(x => !x.MayBeNull).Select(x => x.Name);
            foreach (string column in nonNullableColumnNames) 
                if (!entries.Select(x => x.k).Contains(column)) 
                    throw new Exception($"Column {column} is not marked as MayBeNull, but it doesn't have an entry in {entries.ListNotation()}!");
            foreach ((string k, string v) in entries)
            {
                if (v is null)
                {
                    if (columns.IsKey(k)) throw new Exception($"Column {k} is the key column, but its value is null!");
                    if (!columns.MayBeNull(k)) throw new Exception($"Column {k} is not marked as MayBeNull, but has a null value!");
                }
                // v cannot be null if IsKey() because an exception is thrown above if so
                if (columns.IsKey(k)) Key = v!;
                columnValues[k] = v;
            }
            Columns = columns;
        }
        /// <summary>
        /// Creates a new <c>TsvRow</c> from an unparsed tab-separated string with values in the same order as the columns.
        /// </summary>
        /// <param name="columns"><inheritdoc cref="Columns" path="/summary"/></param>
        /// <param name="unparsed">An unparsed string containing a TSV-format row.</param>
        public TsvRow(ColumnInfoSet columns, string unparsed) : this(columns, columns.Names.Zip(unparsed.Split('\t')).ToArray()) { }
        /// <param name="key">The name of the column whose value to get.</param>
        /// <returns>The value of the column named by the key.</returns>
        public string? this[string key] => columnValues[key];
        /// <summary>
        /// Gets the values of the specified columns in the order they are specified.
        /// </summary>
        /// <param name="keys"></param>
        /// <returns>The values of the columns named in <c>keys</c>, in the order they are named.</returns>
        public IEnumerable<string?> this[IEnumerable<string> keys]
        {
            get
            {
                foreach (string key in keys) yield return this[key];
            }
        }
    }
}
