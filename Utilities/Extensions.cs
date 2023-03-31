using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace grp
{    
    /// <summary>
    /// Contains various utility extensions.
    /// </summary>
    public static class Extensions
    {
        /// <typeparam name="T">The type of the objects to print.</typeparam>
        /// <param name="values">An enumerable holding the objects to print paired with the width of their respective columns.</param>
        /// <returns>A string corresponding to the values <c>t</c> in order, in columns padded to their respective <c>width</c>.</returns>
        public static string InColumns<T>(this IEnumerable<(T t, int width)> values) 
            => Utils.InColumns(values.ToArray());
        /// <typeparam name="T">The type of the objects to print.</typeparam>
        /// <param name="values">An enumerable holding the objects to print.</param>
        /// <param name="widths">An enumerable holding the widths of the columns, which will be applied in the same order as the objects.</param>
        /// <returns>A string corresponding to the <c>values</c> in order, in columns padded to their respective <c>widths</c>.</returns>
        public static string InColumns<T>(this IEnumerable<T> values, IEnumerable<int> widths) => InColumns(values.Zip(widths));
        /// <summary>
        /// Represents an enumerable in human-readable format.
        /// </summary>
        /// <typeparam name="T">The type the <c>enumerable</c> contains.</typeparam>
        /// <param name="enumerable">The enumerable to print.</param>
        /// <returns>A string of the format <c>[item1, item2, ... itemN]</c> representing the items in <c>enumerable</c>.</returns>
        public static string ListNotation<T>(this IEnumerable<T> enumerable) 
            => $"[{enumerable.Select(x => x.PrintNullable()).Aggregate((a, b) => $"{a}, {b}")}]";
        /// <summary>
        /// Represents an object in human-readable format, even if it's <see langword="null"/>.
        /// </summary>
        /// <param name="obj">The object or <see langword="null"/> value to represent.</param>
        /// <param name="ifNull">The string to print if <c>obj</c> is null.</param>
        /// <returns>A string which is either <c>obj.ToString()</c>, if <c>obj</c> is not <see langword="null"/>, or <c>ifNull</c> otherwise.</returns>
        public static string PrintNullable(this object? obj, string ifNull = "(null (not Null))") => obj?.ToString() ?? ifNull;
        /// <summary>
        /// Repeats a character a specified number of times.
        /// </summary>
        /// <param name="c">The character to repeat.</param>
        /// <param name="times">How many of the character should be produced.</param>
        /// <returns>A string which is <c>times</c> instances of <c>c</c>.</returns>
        public static string Repeated(this char c, int times)
        {
            string result = "";
            for (int i = 0; i < times; i++) result += c;
            return result;
        }
        public static string InImageFolder(this string relativePath) => Path.Join(Paths.ImageFolder, relativePath);        
        public static void SaveTo(this Image img, string path)
        {
            using FileStream fs = File.Open(path.InImageFolder(), FileMode.Create);
            img.Save(fs, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
        }
        public static void MutateAndSaveTo(this Image img, Action<IImageProcessingContext> mutation, string path)
        {
            img.Mutate(mutation);
            img.SaveTo(path);
        }
        public static IEnumerable<Image> LoadImages(this IEnumerable<string> paths) => Utils.LoadImages(paths.ToArray());
        public static Image Merge(this IEnumerable<Image> images) => Utils.Merge(images.ToArray());
        public static string FileName(this TsvRow row) => $"{row["discord id"]!}{Path.GetExtension(row["url"]!)}";
        public static string WithoutQuotes(this string s) => s.Replace(""+'"',"");
        public static Image Mask(this Image image, IEnumerable<(int x, int y)> points)
        {
            Image<Rgba32> rgbImage = image.CloneAs<Rgba32>();
            foreach ((int x, int y) in points) rgbImage[x, y] = Color.Transparent;
            return rgbImage;
        }
        public static Image Autocrop(this Image image)
        {
            Image<Rgba32> rgbImg = image.CloneAs<Rgba32>();
            List<(int min, int max)> rowBounds = new();            
            int minY = 0, maxY = 0;
            rgbImg.ProcessPixelRows(accessor =>
            {
                bool encounteredNonEmptyRow = false;
                bool[] rowEmptiness = new bool[accessor.Height];
                for (int y = 0; y < accessor.Height; y++)
                {
                    bool encounteredNonEmptyPixel = false;
                    int minX = 0, maxX = 0;
                    Span<Rgba32> pixelRow = accessor.GetRowSpan(y);
                    for(int x = 0; x < pixelRow.Length; x++)
                    {
                        Rgba32 pixel = pixelRow[x];
                        if(pixel.IsEmpty())
                        {
                            if (!encounteredNonEmptyPixel) minX = x;
                            else if (!pixelRow[x - 1].IsEmpty()) maxX = x;
                        } 
                        else
                        {
                            encounteredNonEmptyPixel = true;
                        }
                    }
                    rowBounds.Add((minX, maxX));
                    rowEmptiness[y] = !encounteredNonEmptyPixel;
                    if (!encounteredNonEmptyPixel)
                    {
                        if (!encounteredNonEmptyRow) minY = y;
                        else if (!rowEmptiness[y - 1]) maxY = y;
                    } else
                    {
                        encounteredNonEmptyRow = true;
                    }
                }
            });
            int minX = rowBounds.Select(x => x.min).Min();
            int maxX = rowBounds.Select(x => x.max).Max();
            Rectangle rect = new(minX, minY, maxX - minX, maxY - minY);
            rgbImg.Mutate((context) => context.Crop(rect));
            return rgbImg;
        }
        public static bool IsEmpty(this Rgba32 pixel) => pixel.A == 0;
    }
}
