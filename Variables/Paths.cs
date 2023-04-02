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
        /// <summary>
        /// The absolute path in which this program will save its data.
        /// </summary>
        public static readonly string BaseFolder = "C:/Users/dninemfive/Documents/workspaces/misc/grp";
        /// <summary>
        /// The folder in which the images will be saved within the base folder.
        /// </summary>
        public static readonly string ImageFolder = Path.Join(BaseFolder, "images");
        /// <summary>
        /// The path to the TSV file which will be read to parse data from.
        /// </summary>
        public static readonly string DataFile = Path.Join(BaseFolder, "grp responses.tsv");
        /// <summary>
        /// The path to the image used to mask out the watermark from each picrew in order to properly join them together.
        /// </summary>
        public static readonly string WatermarkForSubtraction = Path.Join(BaseFolder, "watermark.png");
        /// <summary>
        /// The path to the image which will be appended to credit the picrew author and the group for which the image was generated.
        /// </summary>
        public static readonly string WatermarkForAddition = Path.Join(BaseFolder, "watermark_autocropped.png");
        public static readonly string GoogleAuth = Path.Join(BaseFolder, "google auth.json.secret");
        public static readonly string Password = Path.Join(BaseFolder, "password.txt.secret");
        public static readonly string FileId = Path.Join(BaseFolder, "file id.txt.secret");
    }
}
