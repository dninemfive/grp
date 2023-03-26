using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    internal class User
    {
        public (string name, int discriminator) Id { get; private set; }
        public string UniqueName => $"{Id.name}#{Id.discriminator}";
        public string Name { get; private set; }
        public User(string discordId, string picrewUrl, string? displayName, string? height)
        {
            string[] split = discordId.Split("#");
            Id = (split[0], int.Parse(split[1]));
            Name = displayName ?? Id.name;
        }
    }
}
