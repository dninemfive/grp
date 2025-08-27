using d9.utl;
using System.Collections;

namespace d9.grp;

/// <summary>
/// Collects a set of <see cref="ColumnInfo"/> s into a valid descriptor for a <see
/// cref="TsvDocument"/>, performing basic checks in a centralized place.
/// </summary>
public record ColumnInfoSet : IEnumerable<ColumnInfo>
{
    /// <summary>
    /// The names of the columns in their canonical order.
    /// </summary>
    private readonly List<string> _order = new();
    /// <summary>
    /// The <see cref="ColumnInfo">descriptors</see> for each column indexed by their <see cref="ColumnInfo.Name">Name</see>.
    /// </summary>
    private readonly Dictionary<string, ColumnInfo> _columnInfos = new();
    /// <summary>
    /// Assembles a <c>ColumnInfoSet</c> from an arbitrary number of specified <see
    /// cref="ColumnInfo"/> s. Throws an <see cref="Exception"/> with some information if the column
    /// descriptors conflict, i.e.:
    /// <list type="bullet">
    /// <item>There are 0 or multiple <see cref="ColumnType.Key">Key</see> columns, or</item>
    /// <item>There are multiple columns with the same <see cref="ColumnInfo.Name">Name</see>.</item>
    /// </list>
    /// </summary>
    /// <param name="columnInfos">
    /// The <see cref="ColumnInfo"/> s from which to construct the set. Because of the <c>params</c>
    /// keyword, this does not need to be an explicitly constructed array.
    /// </param>
    /// <remarks>
    /// See the implicit conversion operators in <see cref="ColumnInfo"/>, which may make defining
    /// these more readable.
    /// </remarks>
    /// <exception cref="Exception">Whatever i write here doesn't seem to show up, unfortunately.</exception>
    public ColumnInfoSet(params ColumnInfo[] columnInfos)
    {
        int keyCount = columnInfos.Where(x => x.IsKey).Count();
        if (keyCount != 1)
            throw new Exception($"A ColumnInfoSet must have precisely 1 key column, not {keyCount}!");
        foreach (ColumnInfo ci in columnInfos)
        {
            int nameCount = columnInfos.Where(x => x.Name == ci.Name).Count();
            if (nameCount > 1)
                throw new Exception($"A ColumnInfoSet cannot have duplicate column names, but there are {nameCount} columns with {ci.Name}!");
            _columnInfos[ci.Name] = ci;
        }
        _order = columnInfos.Select(x => x.Name).ToList();
    }
    /// <param name="name">
    /// The <see cref="ColumnInfo.Name">Name</see> of the column whose info will be returned.
    /// </param>
    /// <returns>The <see cref="ColumnInfo"/> corresponding to the column with that name.</returns>
    public ColumnInfo this[string name] => _columnInfos[name];
    /// <param name="column">
    /// The <see cref="ColumnInfo.Name">Name</see> of the column whose <c>Key</c> status to check.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the specified column is a <see cref="ColumnType.Key">Key</see>
    /// column, or <see langword="false"/> otherwise.
    /// </returns>
    public bool IsKey(string column) => this[column].IsKey;
    /// <param name="column">
    /// The <see cref="ColumnInfo.Name">Name</see> of the column whose nullability to check.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the specified column <see cref="ColumnType.Nullable">may be
    /// null</see>, or <see langword="false"/> otherwise.
    /// </returns>
    public bool MayBeNull(string column) => this[column].MayBeNull;
    /// <summary>
    /// Gets the <see cref="ColumnInfo"/> s in this set in their canonical order.
    /// </summary>
    public IEnumerable<ColumnInfo> InOrder => _order.Select(x => this[x]);
    /// <summary>
    /// Gets the names of the columns in this set in their canonical order.
    /// </summary>
    public IEnumerable<string> Names => _order;
    /// <summary>
    /// The number of columns in this set.
    /// </summary>
    public int Count => _order.Count;
    /// <returns>
    /// A short summary of this <c>ColumnInfoSet</c>, i.e. its <see cref="Header"/> representation.
    /// </returns>
    public override string ToString() => $"ColumnInfoSet {Header.ListNotation()}";
    /// <summary>
    /// Gets an enumerator over this <see cref="ColumnInfo"/>, or more precisely its <see
    /// cref="InOrder">InOrder</see> property.
    /// </summary>
    /// <returns>
    /// An enumerator listing the <see cref="ColumnInfo"/> s in this set in their canonical order.
    /// </returns>
    public IEnumerator<ColumnInfo> GetEnumerator() => InOrder.GetEnumerator();
    /// <inheritdoc cref="GetEnumerator"/>
    IEnumerator IEnumerable.GetEnumerator() => InOrder.GetEnumerator();
    /// <summary>
    /// The columns of this set as shown when displaying a table using it, and their widths to use
    /// in that scenario. The <see cref="ColumnType.Key">Key</see> column has `🔑` prepended, and
    /// <see cref="ColumnType.Nullable">Nullable</see> columns have `❓` appended.
    /// </summary>
    public IEnumerable<(string column, int width)> Header
    {
        get
        {
            foreach (ColumnInfo ci in InOrder)
            {
                yield return (ci.Type switch
                {
                    ColumnType.Key => $"🔑{ci.Name}",
                    ColumnType.Nullable => $"{ci.Name}❓",
                    _ => ci.Name
                }, ci.Width);
            }
        }
    }
}