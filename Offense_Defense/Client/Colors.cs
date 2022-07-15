using System.Collections.Generic;
using CitizenFX.Core;

namespace OffenseDefense.Client
{
    class Colors
    {
        private static Color blue = new Color(0, 0, 255);
        private static Color red = new Color(255, 0, 0);
        private static Color green = new Color(0, 255, 0);
        private static Color orange = new Color(255, 165, 0);
        private static Color yellow = new Color(255, 255, 0);
        private static Color pink = new Color(255, 192, 203);
        private static Color purple = new Color(128, 0, 128);
        private static Color white = new Color(255, 255, 255);

        public static Dictionary<string, Color> list = new Dictionary<string, Color>() {
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
            foreach (KeyValuePair<string, Color> kvp in list)
            {
                Debug.WriteLine(string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value));
            }
        }
    }
}
