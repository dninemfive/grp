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
        private const char TAB = '\t';
        public readonly (string name, int width)[] Columns;
        public IEnumerable<string> ColumnNames => Columns.Select(x => x.name);
        public IEnumerable<int> ColumnWidths => Columns.Select(x => x.width);
        public Dictionary<string, TsvRow> Data;
        public Dictionary<string, IEnumerable<string>> Column;
        public TsvDocument(params (string name, int width)[] columns)
        {
            Columns = columns;
        }
        public override string ToString()
        {
            string result = "TsvDocument [";
            foreach (string s in ColumnNames) result += $"{s},";
            result = result[..-1];
            result += $"]({Columns.Length})";
            return result;
        }
        public IEnumerable<string> Readable
        {
            get
            {
                yield return Columns.Readable();
                yield return Columns.Select(x => ('='.Repeated(x.name.Length), x.width)).Readable();
                foreach (TsvRow line in Data.OrderBy(x => x.Key).Select(x => x.Value)) yield return line[ColumnNames].Readable(ColumnWidths);
            }
        }
    }
    internal class TsvRow
    {
        private readonly Dictionary<string, string?> columnValues = new();
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
