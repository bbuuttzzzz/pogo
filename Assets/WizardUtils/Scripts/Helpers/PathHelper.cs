using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WizardUtils
{
    public static class PathHelper
    {
        public static string GetParentDirectory(string path)
        {
            int indexOfLast = path.LastIndexOf(Path.DirectorySeparatorChar);
            if (indexOfLast == -1)
            {
                throw new ArgumentException("Path has no directory seperators");
            }
            return path[..indexOfLast];
        }
    }
}
