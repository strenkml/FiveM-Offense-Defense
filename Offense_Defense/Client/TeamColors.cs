using System.Collections.Generic;
using CitizenFX.Core;

namespace OffenseDefense.Client
{
    class TeamColors
    {
        // Primary
        private static TeamColor blue = new TeamColor(0, 0, 255);
        private static TeamColor red = new TeamColor(255, 0, 0);
        private static TeamColor green = new TeamColor(0, 255, 0);
        private static TeamColor orange = new TeamColor(255, 140, 0);
        private static TeamColor yellow = new TeamColor(255, 255, 0);
        private static TeamColor pink = new TeamColor(255, 145, 175);
        private static TeamColor purple = new TeamColor(128, 0, 128);
        private static TeamColor white = new TeamColor(255, 255, 255);

        // Secondary
        private static TeamColor lightBlue = new TeamColor(102, 102, 255);
        private static TeamColor lightRed = new TeamColor(255, 102, 102);
        private static TeamColor lightGreen = new TeamColor(102, 255, 102);
        private static TeamColor lightOrange = new TeamColor(255, 186, 102);
        private static TeamColor lightYellow = new TeamColor(255, 255, 102);
        private static TeamColor lightPink = new TeamColor(255, 189, 207);
        private static TeamColor lightPurple = new TeamColor(178, 102, 178);
        private static TeamColor lightWhite = new TeamColor(153, 153, 153);

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

        public static Dictionary<string, TeamColor> secondaryList = new Dictionary<string, TeamColor>() {
            {"blue", lightBlue},
            {"red", lightRed},
            {"green", lightGreen},
            {"orange", lightOrange},
            {"yellow", lightYellow},
            {"pink", lightPink},
            {"purple", lightPurple},
            {"white", lightWhite}
        };


        public static bool IsColor(string color)
        {
            return list.ContainsKey(color);
        }

        public static void PrintColors()
        {
            foreach (KeyValuePair<string, TeamColor> kvp in list)
            {
                Debug.WriteLine(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
            }
        }

        public static TeamColor GetTeamColorFromName(string color)
        {
            return list[color];
        }

        public static TeamColor GetLightTeamColorFromName(string color)
        {
            return secondaryList[color];
        }
    }
}
