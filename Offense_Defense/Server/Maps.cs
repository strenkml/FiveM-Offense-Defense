using System.Collections.Generic;
using CitizenFX.Core;

namespace OffenseDefense.Server
{
    class Maps
    {
        private static Map map1 = new Map("Map 1",
        new Vector3(-1435.734f, 797.371f, 183.721f), 0.0f,
        new Vector3(-1435.734f, 797.371f, 183.721f), 0.0f,
        new List<Vector3>() { new Vector3(-1454.072f, 856.981f, 183.720f - 1f), new Vector3(-1526.266f, 940.338f, 167.508f - 1f) });

        public static List<Map> list = new List<Map>() { map1 };

        public static List<string> GetMapNames()
        {
            List<string> mapNames = new List<string>();

            foreach (Map m in list)
            {
                mapNames.Add(m.GetName());
            }
            return mapNames;
        }

        public static Map GetMapFromName(string name)
        {
            return list.Find(e => e.GetName() == name);
        }
    }
}