using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    internal class TsvDocument
    {
        private readonly ColumnInfoSet _columns;
        public ColumnInfoSet Columns => _columns;
        public IEnumerable<string> ColumnNames => Columns.InOrder.Select(x => x.Name);
        public IEnumerable<int> ColumnWidths => Columns.InOrder.Select(x => x.Width);
        public Dictionary<string, TsvRow> Data = new();
        public TsvDocument(ColumnInfoSet columns, IEnumerable<string> data)
        {
            _columns = columns;
            foreach (string s in data) Add(new TsvRow(columns, s));
        }
        public TsvRow this[string key] => Data[key];
        public void Add(TsvRow row)
        {
            if (row.Columns != Columns) throw new Exception($"Row {row}'s columns do not match table {this}'s!");
            Data[row.Key] = row;
        }
        public void Add(IEnumerable<TsvRow> rows)
        {
            foreach (TsvRow row in rows) Add(row);
        }
        public override string ToString()
        {
            string result = "TsvDocument [";
            foreach (string s in ColumnNames) result += $"{s},";
            result = result[..-1];
            result += $"]({_columns.Count})";
            return result;
        }
        public IEnumerable<string> Readable
        {
            get
            {
                yield return Columns.InOrder.Readable();
                yield return Columns.InOrder.Select(x => ('='.Repeated(x.Name.Length), x.Width)).Readable();
                foreach (TsvRow line in Data.OrderBy(x => x.Key).Select(x => x.Value)) yield return line[ColumnNames].Readable(ColumnWidths);
            }
        }
    }
    internal class TsvRow
    {
        private readonly Dictionary<string, string?> columnValues = new();
        // should never have this value because of validation on ColumnInfoSet
        public string Key { get; } = "no key found";
        public ColumnInfoSet Columns { get; }
        public TsvRow(ColumnInfoSet columns, params (string k, string v)[] entries)
        {
            foreach((string k, string v) in entries)
            {
                if(v is null)
                {
                    if (columns.IsKey(k)) throw new Exception($"Column {k} is the key column, but its value is null!");
                    if(!columns.MayBeNull(k)) throw new Exception($"Column {k} is not marked as MayBeNull, but has a null value!");
                }
                // v cannot be null if IsKey() because an exception is thrown above if so
                if (columns.IsKey(k)) Key = v!;
                columnValues[k] = v;
            }
            Columns = columns;
        }
        public TsvRow(ColumnInfoSet columns, string unparsed) : this(columns, columns.Names.Zip(unparsed.Split('\t')).ToArray()) { }
        public string? this[string key] => columnValues[key];
        public IEnumerable<string?> this[IEnumerable<string> keys]
        {
            get
            {
                foreach (string key in keys) yield return this[key];
            }
        }
    }
    public enum ColumnType { Normal, Key, Nullable }
    public record ColumnInfo
    {
        public string Name { get; }        
        public int Width { get; }
        public ColumnType Type { get; }
        public bool IsKey => Type is ColumnType.Key;
        public bool MayBeNull => Type is ColumnType.Nullable;
        public ColumnInfo(string name, int width = 24, ColumnType type = ColumnType.Normal)
        {
            Name = name;
            Width = width;
            Type = type;
        }
        public static implicit operator ColumnInfo(string s) => new(s);
        public static implicit operator ColumnInfo((string name, int width) tuple) => new(tuple.name, tuple.width);
        public static implicit operator ColumnInfo((string name, int width, ColumnType type) tuple) => new(tuple.name, tuple.width, tuple.type);
    }
    public record ColumnInfoSet
    {
        private readonly List<string> _order = new();
        private readonly Dictionary<string, ColumnInfo> _columnInfos = new();
        public ColumnInfoSet(params ColumnInfo[] columnInfos)
        {
            int keyCount = columnInfos.Where(x => x.IsKey).Count();
            if (keyCount != 1) throw new Exception($"A ColumnInfoSet must have precisely 1 key column, not {keyCount}!");
            foreach (ColumnInfo ci in columnInfos)
            {
                int nameCount = columnInfos.Where(x => x.Name == ci.Name).Count();
                if (nameCount > 1) throw new Exception($"A ColumnInfoSet cannot have duplicate column names, but there are {nameCount} columns with {ci.Name}!");
                _columnInfos[ci.Name] = ci;
            }
            _order = columnInfos.Select(x => x.Name).ToList();
        }
        public ColumnInfo this[string k] => _columnInfos[k];
        public bool IsKey(string column) => this[column].IsKey;
        public bool MayBeNull(string column) => this[column].MayBeNull;
        public IEnumerable<ColumnInfo> InOrder => _order.Select(x => this[x]);
        public IEnumerable<string> Names => _order;
        public int Count => _order.Count;
    }
}
