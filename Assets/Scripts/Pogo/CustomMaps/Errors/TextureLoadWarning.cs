using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BSPImporter.BSPLoader;

namespace Pogo.CustomMaps.Errors
{
    public class TextureLoadWarning : MapError
    {
        public TextureLoadWarning(Exception exception, string textureName, string failReason = null) : base(exception, GetMessage(textureName, failReason), MapError.Severities.Warning)
        {

        }

        private static string GetMessage(string textureName, string failReason = null)
        {
            return "Texture '" + textureName + "' could not be found. Are you missing a .wad?";
        }
    }
}
