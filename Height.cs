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
        public float Centimeters { get; private set; }
        public static readonly Height Default = new(6, 0);
        public Height(int feet, int inches)
        {
            int totalInches = feet * 12 + inches;
            Centimeters = totalInches * 2.54f;
        }
        public static Height Parse(string s)
        {
            if(Regex.IsMatch(s, @"\d+'\d+" + '"'))
            {
                string[] split = s.Split("'");
                int feet = int.Parse(split.First());
                int inches = int.Parse(split[1][..-1]);
                return new(feet, inches);
            }
            return Default;
        }
        public override bool Equals(object? obj) => obj is Height h && h.Centimeters == Centimeters;
        public override int GetHashCode() => Centimeters.GetHashCode();
        public static bool operator ==(Height a, Height b) => a.Equals(b);
        public static bool operator !=(Height a, Height b) => !(a == b);
    }
}
