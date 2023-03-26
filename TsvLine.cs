using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    internal class TsvLine
    {
        string Id;
        string? Name;
        string Url;
        string Height;
        public TsvLine(string id, string? name, string url, string height)
        {
            Id = id;
            Name = name;
            Url = url;
            Height = height;
        }
        public IEnumerable<string> Columns
        {
            get
            {
                yield return Id;
                yield return Name ?? "(actually null)";
                yield return Height;
                yield return Url;
            }
        }
        public static implicit operator TsvLine((string id, string? name, string url, string height) tuple)
            => new(tuple.id, tuple.name, tuple.url, tuple.height);
        public static implicit operator (string id, string? name, string url, string height)(TsvLine line) 
            => (line.Id, line.Name, line.Url, line.Height);
    }
}
