using System.Collections.Generic;

namespace OffenseDefense.Server
{
    class Maps
    {
        // private static Map map1 = new Map();

        public static List<Map> list = new List<Map>() { };

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