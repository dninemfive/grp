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
        private readonly List<ColumnInfo> _columns;
        public IEnumerable<ColumnInfo> Columns => _columns;
        public IEnumerable<string> ColumnNames => Columns.Select(x => x.Name);
        public IEnumerable<int> ColumnWidths => Columns.Select(x => x.Width);
        public Dictionary<string, TsvRow> Data = new();
        public TsvDocument(IEnumerable<ColumnInfo> columns, IEnumerable<string> data)
        {
            _columns = columns.ToList();
        }
        public TsvRow this[string key] => Data[key];
        public void Add(TsvRow row)
        {

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
                yield return Columns.Readable();
                yield return Columns.Select(x => ('='.Repeated(x.Name.Length), x.Width)).Readable();
                foreach (TsvRow line in Data.OrderBy(x => x.Key).Select(x => x.Value)) yield return line[ColumnNames].Readable(ColumnWidths);
            }
        }
    }
    internal class TsvRow
    {
        private readonly Dictionary<string, string?> columnValues = new();
        public string Key { get; }
        public TsvRow(params (ColumnInfo k, string v)[] entries)
        {
            if (entries.Select(x => x.k).Where(x => x.IsKey).Count() != 1) throw new Exception($"Tried to initialize TsvRow");
            foreach((ColumnInfo k, string v) in entries)
            {
                if (!k.MayBeNull && v is null) throw new Exception($"Column {k.Name} is not marked as nullable, but has a null value!");
            }
        }
        public string? this[string key] => columnValues[key];
        public IEnumerable<string?> this[IEnumerable<string> keys]
        {
            get
            {
                foreach (string key in keys) yield return this[key];
            }
        }
    }
    public record ColumnInfo
    {
        public string Name { get; }
        public int Width { get; }
        public bool IsKey { get; }
        public bool MayBeNull { get; }
        public ColumnInfo(string name, int width = 24, bool key = false, bool mayBeNull = false)
        {
            Name = name;
            Width = width;
            IsKey = key;
            MayBeNull = mayBeNull;
        }
        public static implicit operator ColumnInfo(string s) => new(s);
        public static implicit operator ColumnInfo((string name, int width) tuple) => new(tuple.name, tuple.width);
        public static implicit operator (string t, int width)(ColumnInfo ci) => (ci.Name, ci.Width);
    }
}
