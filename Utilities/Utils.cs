using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    /// <summary>
    /// Miscellaneous utility methods.
    /// </summary>
    public static class MiscUtils
    {
        /// <summary>
        /// Breaks a collection into <c>n</c> parts of roughly equal size.
        /// </summary>
        /// <remarks>Does not modify the original.</remarks>
        /// <typeparam name="T">The type the enumerable to break up holds.</typeparam>
        /// <param name="original">The enumerable to be broken up.</param>
        /// <param name="n">The number of parts to break the enumerable into.</param>
        /// <returns>An enumerable of enumerables, broken up as described above.</returns>
        public static IEnumerable<IEnumerable<T>> BreakInto<T>(this IEnumerable<T> original, int n)
        {
            int partSize = original.Count() / n;
            int remainder = original.Count() - (n * partSize);
            int ct = 0;
            for (int i = 0; i < n; i++)
            {
                int thisSize = partSize + (remainder-- > 0 ? 1 : 0);
                int endSize = original.Count() - thisSize - ct;
                yield return original.Skip(ct).SkipLast(endSize);
                ct += thisSize;
            }
        }
        /// <summary>
        /// Gets the mean of an arbitrary set of <see langword="float"/>s.
        /// </summary>
        /// <param name="numbers">An array of <see langword="float"/>s to be averaged.</param>
        /// <returns>The <see href="https://en.wikipedia.org/wiki/Arithmetic_mean">arithmetic mean</see> of the given <c>numbers</c>.</returns>
        public static float Mean(params float[] numbers) => numbers.Aggregate((x, y) => x + y) / numbers.Length;
        public static T Clamp<T>(this T t, T min, T max) where T : IComparable
        {
            if (t.CompareTo(min) < 0) return min;
            if (t.CompareTo(max) > 0) return max;
            return t;
        }
    }
}
