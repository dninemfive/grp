using d9.utl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace d9.grp
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
        public static string BaseFolder => Config.BaseFolderPath;
        /// <summary>
        /// The folder in which the images will be saved within the base folder.
        /// </summary>
        public static string ImageFolder => GrpConfig.Current.Paths.ImageFolder.AbsolutePath();
        public static string DebugFolder => GrpConfig.Current.Paths.DebugFolder.AbsolutePath();
        /// <summary>
        /// The path to the TSV file which will be read to parse data from.
        /// </summary>
        public static string TsvFile => GrpConfig.Current.Paths.TsvFile.AbsolutePath();
        /// <summary>
        /// The path to the image used to mask out the watermark from each picrew in order to properly join them together.
        /// </summary>
        public static string WatermarkToSubtract => GrpConfig.Current.Paths.WatermarkToSubtract.AbsolutePath();
        /// <summary>
        /// The path to the image which will be appended to credit the picrew author and the group for which the image was generated.
        /// </summary>
        public static string WatermarkToAdd => GrpConfig.Current.Paths.WatermarkToAdd.AbsolutePath();
        public static string AlphaMask => "images/mask.png".AbsolutePath();
    }
}
