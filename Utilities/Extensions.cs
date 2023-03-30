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
        /// <summary>
        /// Splits a string in the format <c>l|r</c>, where <c>l</c> is a string which can be parsed into <c>T</c> by <c>leftParser</c>, <c>|</c>is the <c>delimiter</c>,
        /// and <c>r</c> is a string which can be parsed into <c>U</c> by <c>rightParser</c>.</summary>
        /// <typeparam name="T">The type to be parsed from the part of the string to the left of the specified <c>delimiter</c>.</typeparam>
        /// <typeparam name="U">The type to be parsed from the part of the string to the right of the specified <c>delimiter</c>.</typeparam>
        /// <param name="s">The string to be split and parsed.</param>
        /// <param name="delimiter">The delimiter separating the left and right parts of <c>s</c>.</param>
        /// <param name="leftParser">A function which parses the left part to <c>T</c>.</param>
        /// <param name="rightParser">A function which parses the right part to <c>U</c>.</param>
        /// <returns>A tuple <c>(L, R)</c>, where <c>L</c> and <c>R</c> correspond to the <c>T</c> and <c>U</c> parsed from <c>l</c> and <c>r</c>.</returns>
        /// <exception cref="Exception">Thrown if the string is not in the right format.</exception>
        public static (T, U) SplitAndParse<T, U>(this string s, string delimiter, Func<string, T> leftParser, Func<string, U> rightParser)
        {
            string[] split = s.Split(delimiter);
            if (split.Length != 2) throw new Exception($"String `{s}` should be split by delimiter `{delimiter}` into exactly two parts, " +
                                                       $"but it was split into {split.Length} parts instead.");
            return (leftParser(split[0]), rightParser(split[1]));
        }
        /// <summary>
        /// As <see cref="SplitAndParse{T, U}(string, string, Func{string, T}, Func{string, U})"/>, but <c>T</c> and <c>U</c> are the same type.
        /// </summary>
        /// <typeparam name="T">The type to be parsed from both the left and right side of the string.</typeparam>
        /// <param name="s">The string to be split and parsed.</param>
        /// <param name="delimiter">The delimiter separating the left and right parts of <c>s</c>.</param>
        /// <param name="parser">A function which parses the left and right sides to <c>T</c>.</param>
        /// <returns>A tuple <c>(L, R)</c>, where <c>L</c> and <c>R</c> correspond to the <c>T</c>s parsed from <c>l</c> and <c>r</c>, respectively.</returns>
        public static (T, T) SplitAndParse<T>(this string s, string delimiter, Func<string, T> parser) => s.SplitAndParse(delimiter, parser, parser);
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
