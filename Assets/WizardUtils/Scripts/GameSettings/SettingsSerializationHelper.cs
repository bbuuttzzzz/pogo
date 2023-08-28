using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WizardUtils.GameSettings
{
    public static class SettingsSerializationHelper
    {
        public static void SerializeSettings(string filePath, IEnumerable<Tuple<string, float>> settings)
        {
            using FileStream fs = File.Create(filePath);
            UTF8Encoding encoding = new UTF8Encoding();

            foreach (var setting in settings)
            {
                byte[] bytes = encoding.GetBytes($"{setting.Item1}: {setting.Item2.ToString(CultureInfo.InvariantCulture)}\n");
                fs.Write(bytes);
            }
        }

        public static IEnumerable<Tuple<string, float>> DeserializeSettings(string filePath)
        {
            List<Tuple<string, float>> result = new List<Tuple<string, float>>();

            IEnumerable<string> lines = File.ReadLines(filePath);
            int lineIndex = 0;
            Regex pattern = new Regex("(.+): ([0-9.]+)", RegexOptions.Compiled);
            foreach(var line in lines)
            {
                lineIndex++;
                Match match = pattern.Match(line);
                if (!match.Success)
                {
                    throw new FormatException($"Badly formatted line {lineIndex}: \"{line}\"");
                }

                if (!float.TryParse(match.Groups[2].Value, NumberStyles.Integer | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out float item2))
                {
                    throw new FormatException($"Failed to convert line {lineIndex} value to float: \"{match.Groups[2].Value}\"");
                }

                result.Add(new Tuple<string, float>(match.Groups[1].Value, item2));
            }

            return result;
        }
    }
}
