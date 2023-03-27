using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace grp
{
    public class Height
    {
        public static readonly Regex FeetAndInches = new(@"\d+'\d+" + '"');
        public static readonly Regex Centimeters = new(@"\d+cm");
        // https://ourworldindata.org/human-height#how-does-human-height-vary-across-the-world
        public static readonly Height Default = new(Utils.Mean(171, 159));
        public float InCentimeters { get; private set; }
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
