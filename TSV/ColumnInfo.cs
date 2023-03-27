using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{    
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
}
