using d9.utl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            foreach(string folder in Folders)
            {
                Directory.CreateDirectory(folder);
            }
        }
        public static void CreateFoldersFor(string groupName)
        {
            foreach (string folder in Folders)
            {
                Directory.CreateDirectory(folder.For(groupName));
            }
        }
        public static string For(this string basePath, string name) => Path.Join(basePath, name);
        public static string DataFile(this string name) => Path.ChangeExtension(Path.Join(DataFolder, name), "tsv");
        public static readonly string[] Folders = { BaseFolder, DataFolder, OutputFolder };
        /// <summary>
        /// The absolute path in which this program will save its data.
        /// </summary>
        public static string BaseFolder => Config.BaseFolderPath;
        /// <summary>
        /// The folder in which the images will be saved within the base folder.
        /// </summary>
        public static string OutputFolder => GrpConfig.Current.Paths.OutputFolder.AbsolutePath();
        public static string DebugFolder(this string groupName) => Path.Join(OutputFolder.For(groupName), "debug").AbsolutePath();
        public static string DataFolder => GrpConfig.Current.Paths.DataFolder.AbsolutePath();
        /// <summary>
        /// The path to the image used to mask out the watermark from each picrew in order to properly join them together.
        /// </summary>
        public static string WatermarkToSubtract(GrpConfig.Group group) => group.WatermarkToSubtract?.AbsolutePath() 
                                                                        ?? GrpConfig.Current.Paths.WatermarkToSubtract.AbsolutePath();
        /// <summary>
        /// The path to the image which will be appended to credit the picrew author and the group for which the image was generated.
        /// </summary>
        public static string WatermarkToAdd(GrpConfig.Group group) => group.WatermarkToAdd?.AbsolutePath()
                                                                   ?? GrpConfig.Current.Paths.WatermarkToAdd.AbsolutePath();
        public static string AlphaMask => "images/mask.png".AbsolutePath();
    }
}
