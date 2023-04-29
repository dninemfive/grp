using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using d9.utl;

namespace grp
{
    /// <summary>
    /// Utilities related to printing various values to the console.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Gets a standard debug name for a given file created by a given method name.
        /// </summary>
        /// <param name="methodName">The name of the method creating this debug file.</param>
        /// <param name="hash">A hash vaguely related to the file being created.</param>
        /// <returns>A string which can be used as a filename for the file being created.</returns>
        public static string DebugName(string methodName, int hash) => $"debug/{methodName}_{DateTime.Now.FileNameFormat()}_{hash}.png";
    }
}
