using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace grp
{    
    /// <summary>
    /// Contains various utility extensions.
    /// </summary>
    public static class Extensions
    {
        public static string InColumns<T>(this IEnumerable<(T t, int width)> enumerable) 
            => Utils.Readable(enumerable.ToArray());
        public static string InColumns<T>(this IEnumerable<T> values, IEnumerable<int> widths) => InColumns(values.Zip(widths));
        public static string InColumns(this IEnumerable<ColumnInfo> values) => InColumns(values.Select(x => (x.Name, x.Width)));
        public static string ListNotation<T>(this IEnumerable<T> enumerable) 
            => $"[{enumerable.Select(x => x.PrintNullable()).Aggregate((a, b) => $"{a}, {b}")}]";
        public static string PrintNullable(this object? obj, string ifNull = "(null (not Null))") => obj?.ToString() ?? ifNull;
        public static string Repeated(this char c, int times)
        {
            string result = "";
            for (int i = 0; i < times; i++) result += c;
            return result;
        }
    }
}
