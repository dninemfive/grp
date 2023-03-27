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
        public IEnumerable<TsvRow> Rows => Data.Values.OrderBy(x => x.Key);
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
}
