using Discord.Interactions;

namespace d9.grp.bot;

[Group("picrewgroupphoto", "Commands relating to the Picrew Group Photo feature.")]
public class PicrewGroupPhotoModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("setup", "Sets up this server to use Picrew Group Photo features.")]
    public async Task SetUp()
    {

    }
    [SlashCommand("link", "Posts a link to the picrew used for this server's group photos.")]
    public async Task Link()
    {

    }
    // todo: variant which allows uploading an image. requires admin approval before being added.
    [SlashCommand("update", "Updates your image for the specified picrew.")]
    public async Task Update([Summary(description: "The link to the output image of your completed picrew for this server.")] string? link = null,
                             [Summary(description: "Your height, used to scale your picrew in group photos.")] string? height = null,
                             [Summary(description: "If true, this image will only be used for group photos in this server."
                                                 + "If false, it will be used for this bot in all servers you're in.")] bool serverSpecific = false)
    {

    }
    [SlashCommand("image", "Posts the current group photo for this server.")]
    public async Task Image([Summary(description: "If true, information about how the image was obtained will be provided.")] bool printDebugInfo = false)
    {

    }
}