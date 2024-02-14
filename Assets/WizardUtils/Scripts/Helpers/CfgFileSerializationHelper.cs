using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WizardUtils.Configurations
{
    public static class CfgFileSerializationHelper
    {
        public static void Serialize(string filePath, IEnumerable<KeyValuePair<string, string>> settings)
        {
            using FileStream fs = File.Create(filePath);
            UTF8Encoding encoding = new UTF8Encoding();

            foreach (var setting in settings)
            {
                byte[] bytes = encoding.GetBytes($"{setting.Key}: {setting.Value}\n");
                fs.Write(bytes);
            }
        }

        public static IEnumerable<KeyValuePair<string, string>> Deserialize(string filePath)
        {
            List<KeyValuePair<string, string>> result = new List<KeyValuePair<string, string>>();

            IEnumerable<string> lines = File.ReadLines(filePath);
            int lineIndex = 0;
            Regex pattern = new Regex("(.+): (.+)", RegexOptions.Compiled);
            foreach(var line in lines)
            {
                lineIndex++;
                Match match = pattern.Match(line);
                if (!match.Success)
                {
                    throw new FormatException($"Badly formatted line {lineIndex}: \"{line}\"");
                }

                result.Add(new KeyValuePair<string, string>(match.Groups[1].Value, match.Groups[2].Value));
            }

            return result;
        }
    }
}
