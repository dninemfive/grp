using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    internal class TsvRow
    {
        private readonly Dictionary<string, string?> columnValues = new();
        // should never have this value because of validation on ColumnInfoSet
        public string Key { get; } = "no key found";
        public ColumnInfoSet Columns { get; }
        public TsvRow(ColumnInfoSet columns, params (string k, string v)[] entries)
        {
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
}
