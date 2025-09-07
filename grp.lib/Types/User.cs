using System.Diagnostics.CodeAnalysis;

namespace d9.grp.lib;
public class User(string id, IEnumerable<UserUpdate> updates)
{
    public string Id => id;
    internal IEnumerable<UserUpdate> Updates => updates;

}