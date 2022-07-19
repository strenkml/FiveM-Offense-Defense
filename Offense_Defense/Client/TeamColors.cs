using System.Collections.Generic;
using CitizenFX.Core;

namespace OffenseDefense.Client
{
    class TeamColors
    {
        private static TeamColor blue = new TeamColor(0, 0, 255);
        private static TeamColor red = new TeamColor(255, 0, 0);
        private static TeamColor green = new TeamColor(0, 255, 0);
        private static TeamColor orange = new TeamColor(255, 165, 0);
        private static TeamColor yellow = new TeamColor(255, 255, 0);
        private static TeamColor pink = new TeamColor(255, 192, 203);
        private static TeamColor purple = new TeamColor(128, 0, 128);
        private static TeamColor white = new TeamColor(255, 255, 255);

        public static Dictionary<string, TeamColor> list = new Dictionary<string, TeamColor>() {
            {"blue", blue},
            {"red", red},
            {"green", green},
            {"orange", orange},
            {"yellow", yellow},
            {"pink", pink},
            {"purple", purple},
            {"white", white}
        };

        public static bool IsColor(string color)
        {
            Debug.WriteLine($"Checking color [{color}]");
            return list.ContainsKey(color);
        }

        public static void PrintColors()
        {
            foreach (KeyValuePair<string, TeamColor> kvp in list)
            {
                Debug.WriteLine(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
            }
        }
    }
}
