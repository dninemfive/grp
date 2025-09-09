namespace d9.grp.lib;

/// <summary>
/// Represents a concrete instance of a participant in a group "photo". Contains only information
/// required to render the image and generate a description.
/// </summary>
public record User(string Name, string ImagePath, Height? Height = null);