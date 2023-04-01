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
        public DateTime Timestamp { get; private set; }
        public (string name, int discriminator) Id { get; private set; }
        public string DiscordId => $"{Id.name}#{Id.discriminator.ToString().PadLeft(4, '0')}";
        public string Name { get; private set; }
        public Image? Image { get; private set; }
        public Height Height { get; private set; }
        public string Url { get; private set; }
        private User(TsvRow row)
        {
            DateTime.Parse(row["timestamp"]!);
            string[] split = row["discord id"]!.Split("#");
            Id = (split[0], int.Parse(split[1]));
            Name = row["display name"]! ?? Id.name;
            Url = row["url"]!;
            Height = Height.Parse(row["height"]!);                     
        }
        private async Task GetImage()
        {
            Image = await NetUtils.DownloadImage(Url, FileName);
            if (Image is null) return;
            Image = Image.Mask(Constants.WatermarkMask);
            Image = Image.Autocrop(AutocropType.Vertical);
            Image.Mutate((context) => context.Resize(new ResizeOptions()
            {
                Mode = ResizeMode.Stretch,
                Position = AnchorPositionMode.Bottom,
                Size = (Size)(Image.Size * Height.Ratio)
            }));
            Image.Mutate((context) => context.Resize(new ResizeOptions()
            {
                Mode = ResizeMode.BoxPad,
                Position = AnchorPositionMode.Bottom,
                Size = new(Image.Height, Image.Height)
            }));
        }
        public static async Task<User> Parse(TsvRow row)
        {
            User result = new(row);
            await result.GetImage();
            return result;
        }
        public string FileName => $"{DiscordId}{Path.GetExtension(Url)}";
        public override string ToString() => $"[User {DiscordId,-13} {Name,-8} {Height,-10}]";
    }
}
