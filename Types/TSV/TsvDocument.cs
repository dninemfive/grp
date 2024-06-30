using d9.utl;

namespace d9.grp;

/// <summary>
/// Parses a tabbed-separated-value (TSV) file and represents it in code as a database.
/// </summary>
internal class TsvDocument
{
    /// <summary>
    /// The <see cref="ColumnInfoSet"/> describing this table's columns.
    /// </summary>
    public ColumnInfoSet Columns { get; private set; }
    /// <summary>
    /// The names of this table's columns in their canonical order.
    /// </summary>
    public IEnumerable<string> ColumnNames => Columns.InOrder.Select(x => x.Name);
    /// <summary>
    /// The display widths of this table's columns in their canonical order.
    /// </summary>
    public IEnumerable<int> ColumnWidths => Columns.InOrder.Select(x => x.Width);
    /// <summary>
    /// Stores the rows of this table.
    /// </summary>
    private readonly Dictionary<string, TsvRow> Data = new();
    /// <summary>
    /// The rows in this table.
    /// </summary>
    public IEnumerable<TsvRow> Rows => Data.Values.OrderBy(x => x.Key);
    /// <summary>
    /// Creates a <c>TsvDocument</c> with the specified columns and unparsed data.
    /// </summary>
    /// <param name="columns">The <see cref="ColumnInfoSet"/> describing this document's columns.</param>
    /// <param name="data">
    /// A set of unparsed strings to be converted into <see cref="TsvRow"/> s in this table. If
    /// not specified, the table will be empty at first.
    /// </param>
    public TsvDocument(ColumnInfoSet columns, IEnumerable<string>? data = null)
    {
        Columns = columns;
        if (data is not null)
            foreach (string s in data)
                Add(new TsvRow(columns, s));
    }
    /// <summary>
    /// Gets the <see cref="TsvRow"/> in this table with the specified value in its <see
    /// cref="ColumnType.Key">Key</see> column, if any such row exists.
    /// </summary>
    /// <param name="key">
    /// The value this row has in the <see cref="ColumnType.Key">Key</see> column.
    /// </param>
    /// <returns></returns>
    public TsvRow this[string key] => Data[key];
    /// <summary>
    /// Adds a row to this table, if its columns match.
    /// </summary>
    /// <remarks>Any existing value is silently overwritten.</remarks>
    /// <param name="row">The row to add.</param>
    /// <exception cref="Exception">
    /// Thrown if the row's columns do not match those of this table.
    /// </exception>
    public void Add(TsvRow row)
    {
        if (row.Columns != Columns)
            throw new Exception($"Row {row}'s columns do not match table {this}'s!");
        Data[row.Key] = row;
    }
    /// <summary>
    /// Adds several rows to this table, if their columns match.
    /// </summary>
    /// <remarks>Any existing values are silently overwritten.</remarks>
    /// <param name="rows">The rows to add.</param>
    /// <exception cref="Exception">
    /// thrown if any row's columns do not match those of this table.
    /// </exception>
    public void Add(params TsvRow[] rows)
    {
        foreach (TsvRow row in rows)
            Add(row);
    }
    /// <inheritdoc cref="Add(TsvRow[])"/>
    public void Add(IEnumerable<TsvRow> rows) => Add(rows.ToArray());
    /// <returns>A short summary of this table, namely its column names.</returns>
    public override string ToString()
    {
        string result = "TsvDocument [";
        foreach (string s in ColumnNames)
            result += $"{s},";
        result = result[..-1];
        result += $"]({Columns.Count})";
        return result;
    }
    /// <summary>
    /// All of the rows of this table, under a header describing the columns.
    /// </summary>
    public IEnumerable<string> Readable
    {
        get
        {
            yield return Columns.Header.InColumns();
            yield return Columns.Header.Select(x => ('='.Repeated(x.column.Length), x.width)).InColumns();
            foreach (TsvRow line in Data.OrderBy(x => x.Key).Select(x => x.Value))
                yield return line[ColumnNames].InColumns(ColumnWidths);
        }
    }
}