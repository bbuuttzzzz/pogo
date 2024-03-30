using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pogo.CustomMaps.Indexing
{
    public enum MapTags
    {
        None,
        Kaizo,
        Troll,
        Experimental,
        ShortMap,
        Speedrun,
        FreeRoam,
        C0,
        C1,
        C2,
        C3,
        C4,
        C5
    }

    public static class MapTagsHelper
    {
        public static string[] names =
        {
            null,
            "Kaizo",
            "Troll",
            "Experimental",
            "Short And Sweet",
            "Speedrun",
            "Free Roam",
            "C0",
            "C1",
            "C2",
            "C3",
            "C4",
            "C5",
        };

        public static bool TryParse(string str, out MapTags result)
        {
            int index = Array.IndexOf(names, str);
            if (index == -1)
            {
                result = default;
                return false;
            }
            result = (MapTags)index;
            return true;
        }

        public static MapTags FromString(string str)
        {
            int index = Array.IndexOf(names, str);
            if (index == -1)
            {
                throw new KeyNotFoundException($"Invalid Map Tag '{str}'. Expected tags like 'Kaizo, Short Map, Speedrun'");
            }
            return (MapTags)index;
        }

        public static string ToString(MapTags tag)
        {
            return names[(int)tag];
        }

        public static IList<string> ToSteamTagFormat(MapTags[] tags)
        {
            List<string> result = new List<string>();
            if (tags == null)
            {
                return result;
            }

            foreach(var tag in tags)
            {
                result.Add(ToString(tag));
            }

            return result;
        }
    }
}
