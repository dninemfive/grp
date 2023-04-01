using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    public static class IoUtils
    {
        public static Image LoadImage(this string path)
        {
            if (!Path.IsPathFullyQualified(path)) path = path.InImageFolder();
            Console.WriteLine($"Attempting to load image at {path}...");
            using FileStream fs = File.OpenRead(path);
            return Image.Load(fs);
        }        
        public static IEnumerable<Image> LoadImages(params string[] paths)
        {
            foreach (string path in paths) yield return LoadImage(path);
        }
        public static IEnumerable<Image> LoadImages(this IEnumerable<string> paths) => LoadImages(paths.ToArray());
    }
}
