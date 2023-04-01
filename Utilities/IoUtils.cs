using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    /// <summary>
    /// Utilities relating to saving and loading files.
    /// </summary>
    public static class IoUtils
    {
        /// <summary>
        /// If a given path is relative rather than absolute, returns an absolute path corresponding to that path relative to the 
        /// <see cref="Paths.ImageFolder">default image folder</see>.
        /// </summary>
        /// <param name="path">A relative or absolute path.</param>
        /// <returns>An absolute path corresponding to the <c>relativePath</c> located within the default image folder.</returns>
        public static string AbsoluteOrInImageFolder(this string path) => Path.IsPathFullyQualified(path) switch
        {
            true => path,
            false => Path.Join(Paths.ImageFolder, path)
        };
        /// <summary>
        /// Loads an image at the specified <c>path</c>. If the <c>path</c> is relative, looks for the image in the 
        /// <see cref="Paths.ImageFolder">default image folder</see>.
        /// </summary>
        /// <param name="path">A path to the desired image.</param>
        /// <returns>The image at <c>path</c>, if it exists.</returns>
        public static Image LoadImage(this string path)
        {
            path = path.AbsoluteOrInImageFolder();
            using FileStream fs = File.OpenRead(path);
            return Image.Load(fs);
        }
        /// <summary>
        /// Saves an image to the specified <c>path</c>. If the <c>path</c> is relative, saves it in the 
        /// <see cref="Paths.ImageFolder">default image folder</see>.
        /// </summary>
        /// <remarks>If the file already exists, <b>it will be overwritten</b>.</remarks>
        /// <param name="img">The <see cref="Image"/> to save.</param>
        /// <param name="path">A path to the desired image.</param>
        public static void SaveTo(this Image img, string path)
        {
            using FileStream fs = File.Open(path.AbsoluteOrInImageFolder(), FileMode.Create);
            img.Save(fs, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
        }
    }
}
