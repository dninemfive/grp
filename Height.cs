using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace grp
{
    /// <summary>
    /// Parses a string of in the format <c><em>x</em>'<em>y</em>"</c> or <c><em>z</em>cm</c> into a height in centimeters.
    /// </summary>
    public class Height
    {
        /// <summary>
        /// Matches <c><em>x</em>'<em>y</em>"</c>. 
        /// </summary>
        public static readonly Regex FeetAndInches = new(@"\d+'\d+" + '"');
        /// <summary>
        /// Matches <c><em>z</em>cm</c>.
        /// </summary>
        public static readonly Regex Centimeters = new(@"\d+cm");
        /// <summary>
        /// The default height used to calculate the <see cref="Ratio"/> to be used to scale images.
        /// </summary>
        /// <remarks>Calculated by averaging the amab and afab heights given
        /// <see href="https://ourworldindata.org/human-height#how-does-human-height-vary-across-the-world">here</see>.</remarks>
        public static readonly Height Default = new(Utils.Mean(171, 159));
        /// <summary>
        /// The canonical height parsed, given in centimeters.
        /// </summary>
        public float InCentimeters { get; private set; }
        /// <summary>
        /// The ratio of this height to the <see cref="Default"/>.
        /// </summary>
        public float Ratio => InCentimeters / Default.InCentimeters;
        public Height(int feet, int inches)
        {
            int totalInches = feet * 12 + inches;
            InCentimeters = totalInches * 2.54f;
        }
        public Height(float centimeters)
        {
            InCentimeters = centimeters;
        }
        public static Height Parse(string s)
        {
            if(FeetAndInches.IsMatch(s))
            {
                string[] split = s.Split("'");
                int feet = int.Parse(split.First());
                int inches = int.Parse(split[1][..-1]);
                return new(feet, inches);
            }
            return Default;
        }        
        public override bool Equals(object? obj) => obj is Height h && h.InCentimeters == InCentimeters;
        public override int GetHashCode() => InCentimeters.GetHashCode();
        public static bool operator ==(Height a, Height b) => a.Equals(b);
        public static bool operator !=(Height a, Height b) => !(a == b);
    }
}
