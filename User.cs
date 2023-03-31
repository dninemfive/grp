using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    internal class User
    {
        public (string name, int discriminator) Id { get; private set; }
        public string DiscordId => $"{Id.name}#{Id.discriminator}";
        public string Name { get; private set; }
        public Image? Image { get; private set; }
        public Height Height { get; private set; }
        public string Url { get; private set; }
        private User(TsvRow row)
        {
            string[] split = row["discord id"]!.Split("#");
            Id = (split[0], int.Parse(split[1]));
            Name = row["display name"]! ?? Id.name;
            Url = row["url"]!;            
            Height = Height.Parse(row["height"]!);                     
        }
        private async Task GetImage()
        {
            Image = await Utils.DownloadImage(Url, FileName);
            if (Image is null) return;
            Image.Mutate((context) => context.DrawImage(Constants.WatermarkForSubtraction, PixelColorBlendingMode.Subtract, 1));
            Image.Mutate((context) => context.Resize(new ResizeOptions()
            {
                Mode = ResizeMode.BoxPad,
                Position = AnchorPositionMode.Bottom,
                Size = (Size)(Image.Size * Height.Ratio)
            }));
        }
        public static async Task<User> Parse(TsvRow row)
        {
            User result = new(row);
            await result.GetImage();
            return result;
        }
        public string FileName => $"{DiscordId}{Path.GetExtension(Url)}";
    }
}
