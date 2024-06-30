namespace d9.grp;

/// <summary>
/// Descriptor for a given column in a <see cref="TsvDocument"/>, comparable to a field on an object
/// in a list.
/// </summary>
public record ColumnInfo
{
    /// <summary>
    /// The name of the column, used to select it when inserting and retrieving items.
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// The number of characters of width allotted to this column when printing the table.
    /// </summary>
    public int Width { get; }
    /// <summary>
    /// What type the column is, one of <c>Key</c>, <c>Normal</c>, or <c>Nullable</c>.
    /// </summary>
    /// <remarks>See <see cref="ColumnType"/> for additional details.</remarks>
    public ColumnType Type { get; }
    /// <summary>
    /// Whether this column is a <c>Key</c>, used to index items in the associated <see cref="TsvDocument"/>.
    /// </summary>
    public bool IsKey => Type is ColumnType.Key;
    /// <summary>
    /// Whether this column is <c>Nullable</c>, i.e. whether it may contain <c>null</c> elements.
    /// </summary>
    public bool MayBeNull => Type is ColumnType.Nullable;
    /// <summary>
    /// Constructs a new <c>ColumnInfo</c>.
    /// </summary>
    /// // https://developercommunity.visualstudio.com/t/inheritdoc-tooltip-does-not-work-for-parameter-nam/932737
    /// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
    /// <param name="width"><inheritdoc cref="Width" path="/summary"/></param>
    /// <param name="type"><inheritdoc cref="Type" path="/summary"/></param>
    public ColumnInfo(string name, int width = 24, ColumnType type = ColumnType.Normal)
    {
        Name = name;
        Width = width;
        Type = type;
    }
    /// <summary>
    /// Implicitly converts a <c>string</c> into a <c>ColumnInfo</c> with the same name.
    /// </summary>
    /// <param name="name"><inheritdoc cref="Name" path="/summary"/></param>
    public static implicit operator ColumnInfo(string name) => new(name);
    /// <summary>
    /// Implicitly converts a <c>(string, int)</c> tuple into a <c>ColumnInfo</c> with the string as
    /// its name and the int as its width.
    /// </summary>
    /// <param name="tuple">
    /// <c>(string, int)</c> where:
    /// <list type="bullet">
    /// <item><c>name</c>: <inheritdoc cref="Name" path="/summary"/></item>
    /// <item><c>width</c>: <inheritdoc cref="Width" path="/summary"/></item>
    /// </list>
    /// </param>
    public static implicit operator ColumnInfo((string name, int width) tuple) => new(tuple.name, tuple.width);

    /// <summary>
    /// Implicitly converts a <c>(string, int, <see cref="ColumnType"/>)</c> tuple into a
    /// <c>ColumnInfo</c> with the string as its name, the int as its width, and the ColumnType as
    /// its type.
    /// </summary>
    /// <param name="tuple">
    /// <c>(string, int, <see cref="ColumnType"/>)</c> where:
    /// <list type="bullet">
    /// <item><c>name</c>: <inheritdoc cref="Name" path="/summary"/></item>
    /// <item><c>width</c>: <inheritdoc cref="Width" path="/summary"/></item>
    /// <item><c>type</c>: <inheritdoc cref="Type" path="/summary"/></item>
    /// </list>
    /// </param>
    public static implicit operator ColumnInfo((string name, int width, ColumnType type) tuple) => new(tuple.name, tuple.width, tuple.type);
}