using System.Collections.Generic;
using CitizenFX.Core;

namespace OffenseDefense.Server
{
    class Maps
    {
        private static Map map1 = new Map("first", new Vector3(0.0f), 0.0f, new List<Vector3>() { new Vector3(0.0f) });

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