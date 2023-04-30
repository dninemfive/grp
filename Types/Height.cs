using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using d9.utl;

namespace grp
{
    /// <summary>
    /// Parses a string of in the format <c><em>x</em>'<em>y</em>"</c> or <c><em>z</em>cm</c> into a height in centimeters.
    /// </summary>
    public class Height : IComparable
    {
        /// <summary>
        /// Matches <c><em>x</em>'<em>y</em>"</c>, where <c><em>x</em></c> and <c><em>y</em></c> are numbers.
        /// </summary>
        private static readonly Regex FeetAndInches = new(@$"\d+[{Constants.Apostrophes}]\d+[{Constants.Quotes}]");
        /// <summary>
        /// Matches <c><em>z</em>cm</c>, where <c><em>z</em></c> is a number.
        /// </summary>
        private static readonly Regex Centimeters = new(@"\d+cm");
        /// <summary>
        /// A list of <see cref="Parser"/>s which will be evaluated by <see cref="Parse(string)"/> in sequence to try and construct a height from a string.
        /// </summary>
        private static readonly List<Parser> Parsers = new()
        {
            new(FeetAndInches, delegate(string s)
            {
                string[] split = s.Without(Constants.Quotes).Split(Constants.Apostrophes.ToArray());
                int feet = int.Parse(split[0]);
                int inches = int.Parse(split[1]);
                return new(feet, inches);
            }),
            new(Centimeters, s => new(float.Parse(s.Replace("cm",""))))
        };
        /// <summary>
        /// A wrapper for the logic of parsing things.
        /// </summary>
        /// <remarks>Probably overkill, but i enjoy writing things like this.</remarks>
        class Parser
        {
            /// <summary>
            /// A regex which must be matched in order for the parser to successfully apply itself.
            /// </summary>
            public Regex Regex;
            /// <summary>
            /// A function which takes a <see langword="string"/> and returns a corresponding <see cref="Height"/>.
            /// </summary>
            public Func<string, Height> Parse;
            /// <summary>
            /// Constructs a <see cref="Parser"/>.
            /// </summary>
            /// <param name="regex"><inheritdoc cref="Regex" path="/summary"/></param>
            /// <param name="parser"><inheritdoc cref="Parse" path="/summary"/></param>
            public Parser(Regex regex, Func<string, Height> parser)
            {
                Regex = regex;
                Parse = parser;
            }
            /// <summary>
            /// Compares a string to the <see cref="Regex">Regex</see> and, if the regex applies, sets <c>result</c> to the <see cref="Height"/>
            /// which results when <see cref="Parse">Parse</see>d.
            /// </summary>
            /// <param name="s">The <see langword="string"/> to try to parse.</param>
            /// <param name="result">The resultant <see cref="Height"/> if the parse attempt was successful, or <see langword="null"/> otherwise.</param>
            /// <returns><see langword="true"/> if the parse attempt was successful, or <see langword="false"/> otherwise.</returns>
            public bool TryParse(string s, out Height? result)
            {
                result = null;
                if (!Regex.IsMatch(s)) return false;
                try
                {
                    result = Parse(s);
                    return true;
                } catch(Exception e)
                {
                    Console.WriteLine($"`{s}` matched the regex {Regex} but parsing it failed: {e.Message}");
                    return false;
                }
            }
        }
        /// <summary>
        /// The default height used to calculate the <see cref="Ratio"/> to be used to scale images.
        /// </summary>
        /// <remarks>Calculated by averaging the amab and afab heights given
        /// <see href="https://ourworldindata.org/human-height#how-does-human-height-vary-across-the-world">here</see>.</remarks>
        public static readonly Height Default = new(MathUtils.Mean(171, 159));
        public static readonly Height Minimum = new(2, 0);
        public static readonly Height Maximum = new(9, 0);
        /// <summary>
        /// The canonical height parsed, given in centimeters.
        /// </summary>
        public float InCentimeters { get; private set; }
        /// <summary>
        /// The ratio of this height to the <see cref="Default">Default</see>, used to scale images.
        /// </summary>
        public float Ratio => InCentimeters / Default.InCentimeters;
        /// <summary></summary>
        /// <remarks>The values are not actually bounded, so they don't technically need to meet the normal requirement that `inches` < 12, for example.</remarks>
        /// <param name="feet">How many feet tall the height is.</param>
        /// <param name="inches">The remaining height in inches.</param>
        public Height(int feet, int inches)
        {
            int totalInches = feet * 12 + inches;
            InCentimeters = totalInches * 2.54f;
        }
        public Height(float centimeters)
        {
            InCentimeters = centimeters;
        }
        public override string ToString() => $"{InCentimeters}cm";
        /// <summary>
        /// Tries to parse a <see langword="string"/> into a <see cref="Height"/> via several methods, listed in <see cref="Parsers"/>.
        /// </summary>
        /// <param name="s">The <see langword="string"/> to try to parse.</param>
        /// <returns>A successfully parsed <see cref="Height"/>, or <see cref="Default"/> if no parse attempts were successful.</returns>
        public static Height Parse(string s)
        {
            foreach (Parser parser in Parsers) if (parser.TryParse(s, out Height? result) && result is not null) return result;
            Console.WriteLine($"Failed to parse {s} using any of the existing parsers.");
            return Default;
        }
        /// <summary>
        /// Compares a <see cref="Height"/> to an arbitrary other object for equality.
        /// </summary>
        /// <remarks>A <see cref="Height"/> equals another object if and only if the other object is of type <c>Height</c> 
        /// and both <see cref="InCentimeters">InCentimeters</see> values match.</remarks>
        /// <param name="obj">The <see langword="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the object is equal as described above, or <see langword="false"/> otherwise.</returns>
        public override bool Equals(object? obj) => obj is Height h && h.InCentimeters == InCentimeters;
        /// <summary>
        /// Gets the hash code for this <see cref="Height"/> object. Because equality is determined solely by the <see cref="InCentimeters">InCentimeters</see>
        /// property, its hash code is returned.
        /// </summary>
        /// <returns>A pseudo-unique hash code for this object's value, suitable for use in hash tables and the like.</returns>
        public override int GetHashCode() => InCentimeters.GetHashCode();
        /// <summary>
        /// Implements <see href="https://learn.microsoft.com/en-us/dotnet/api/system.icomparable?view=net-8.0">IComparable</see>.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns>A negative value if this should come before the object, a positive value if it should come after (or the other is <see langword="null"/>),
        /// or zero if they are equal, corresponding precisely to a <see langword="float"/> being compared to <c>obj</c>.</returns>
        /// <exception cref="ArgumentException">Thrown if <c>obj</c> is non-<see langword="null"/> but is neither a <see cref="Height"/>
        /// nor a <see langword="decimal"/>.</exception>
        public int CompareTo(object? obj)
        {
            if (obj is null) return 1;
            if (obj is Height h) return InCentimeters.CompareTo(h.InCentimeters);
            if (obj is decimal d) return InCentimeters.CompareTo(d);
            throw new ArgumentException($"Cannot compare height {this} to {obj.GetType().Name} {obj}!");
        }
        /// <summary>
        /// Implements the equality operator between two <see cref="Height"/> instances. Implemented using <see cref="Equals(object?)"/>, which states that
        /// "<inheritdoc cref="Equals(object?)" path="/remarks"/>"
        /// </summary>
        /// <param name="a">The first <c>Height</c> to compare.</param>
        /// <param name="b">The second <c>Height</c> to compare.</param>
        /// <returns><see langword="true"/> if the <c>Height</c>s are equal as described above, or <see langword="false"/> otherwise.</returns>
        public static bool operator ==(Height a, Height b) => a.Equals(b);
        /// <summary>
        /// Implements the inequality operator between two <see cref="Height"/> instances. Implemented as the inverse of <see cref="operator ==(Height, Height)"/>,
        /// which in turn derives equality from <see cref="Equals(object?)"/>, which defines equality as "<inheritdoc cref="Equals(object?)" path="/remarks"/>"
        /// </summary>
        /// <param name="a">The first <c>Height</c> to compare.</param>
        /// <param name="b">The second <c>Height</c> to compare.</param>
        /// <returns><see langword="true"/> if the <c>Height</c>s are unequal as described above, or <see langword="false"/> otherwise.</returns>
        public static bool operator !=(Height a, Height b) => !(a == b);
        public static bool operator >(Height a, Height b) => a.CompareTo(b) > 0;
        public static bool operator <(Height a, Height b) => a.CompareTo(b) < 0;
    }
}
