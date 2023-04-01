using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    /// <summary>
    /// Non-extension utility methods.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Gets the mean of an arbitrary set of <see langword="float"/>s.
        /// </summary>
        /// <param name="numbers">An array of <see langword="float"/>s to be averaged.</param>
        /// <returns>The <see href="https://en.wikipedia.org/wiki/Arithmetic_mean">arithmetic mean</see> of the given <c>numbers</c>.</returns>
        public static float Mean(params float[] numbers) => numbers.Aggregate((x, y) => x + y) / numbers.Length;
    }
}
