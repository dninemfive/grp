using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
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
