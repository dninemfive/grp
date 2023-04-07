using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    /// <summary>
    /// Useful paths for this program.
    /// </summary>
    public static class Paths
    {
        public static void CreateFolders()
        {
            foreach(string folder in new[]  { BaseFolder, ImageFolder, DebugFolder})
            {
                Directory.CreateDirectory(folder);
            }
        }
        /// <summary>
        /// The absolute path in which this program will save its data.
        /// </summary>
        public static string BaseFolder => Config.Current?.Paths.BaseFolder ?? IoUtils.WorkingDirectory;
        public static readonly string DefaultConfigFile = Path.Join(BaseFolder, "config.json");
        /// <summary>
        /// The folder in which the images will be saved within the base folder.
        /// </summary>
        public static string ImageFolder => Path.Join(BaseFolder, Config.Current.Paths.ImageFolder);
        public static string DebugFolder => Path.Join(BaseFolder, Config.Current.Paths.DebugFolder);
        /// <summary>
        /// The path to the TSV file which will be read to parse data from.
        /// </summary>
        public static string TsvFile => Path.Join(BaseFolder, Config.Current.Paths.TsvFile);
        /// <summary>
        /// The path to the image used to mask out the watermark from each picrew in order to properly join them together.
        /// </summary>
        public static string WatermarkToSubtract => Path.Join(BaseFolder, Config.Current.Paths.WatermarkToSubtract);
        /// <summary>
        /// The path to the image which will be appended to credit the picrew author and the group for which the image was generated.
        /// </summary>
        public static string WatermarkToAdd => Path.Join(BaseFolder, Config.Current.Paths.WatermarkToAdd);
        /// <summary>
        /// The path to the Google config file.
        /// </summary>
        public static string GoogleConfig => Path.Join(BaseFolder, Config.Current.Paths.GoogleAuth);
        /// <summary>
        /// The path to the P12 file used for Google authentication.
        /// </summary>
        public static string GoogleKey => Path.Join(BaseFolder, Config.Current.GoogleAuth?.Key);
        public static string AlphaMask => Path.Join(BaseFolder, "images/mask.png");
    }
}
