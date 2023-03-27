using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grp
{
    public static class Constants
    {
        public const string BASE_PATH = "C:/Users/dninemfive/Documents/workspaces/misc/grp";
        public static readonly string ImageFolderPath = Path.Join(BASE_PATH, "images");
        public static readonly string DataPath = Path.Join(BASE_PATH, "grp responses.tsv");
        public static readonly HttpClient Client = new();
    }
}
