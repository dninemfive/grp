using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace grp
{    
    public static class Extensions
    {
        public static (T, U) SplitAndParse<T, U>(this string s, string delimiter, Func<string, T> leftParser, Func<string, U> rightParser)
        {
            string[] split = s.Split(delimiter);
            if (split.Length != 2) throw new Exception($"String `{s}` should be split by delimiter `{delimiter}` into exactly two parts, " +
                                                       $"but it was split into {split.Length} parts instead.");
            return (leftParser(split[0]), rightParser(split[1]));
        }
        public static (T, T) SplitAndParse<T>(this string s, string delimiter, Func<string, T> parser) => s.SplitAndParse(delimiter, parser, parser);
        public static string Readable<T>(this IEnumerable<(T t, int width)> enumerable) 
            => Utils.Readable(enumerable.ToArray());
        public static string Readable<T>(this IEnumerable<T> values, IEnumerable<int> widths) => Readable(values.Zip(widths));
        public static string Readable(this IEnumerable<ColumnInfo> values) => Readable(values.Select(x => (x.Name, x.Width)));
        public static string Repeated(this char c, int times)
        {
            string result = "";
            for (int i = 0; i < times; i++) result += c;
            return result;
        }
    }
}
