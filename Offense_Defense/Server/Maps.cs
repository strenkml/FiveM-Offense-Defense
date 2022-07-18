using System.Collections.Generic;

namespace OffenseDefense.Server
{
    class Maps
    {
        private static Map map1 = new Map("", "", 14);

        public static List<Map> list = new List<Map>() { map1 };

        public static List<string> GetMapNames()
        {
            List<string> mapNames = new List<string>();

            foreach (Map m in this.list)
            {
                mapNames.Add(m.GetName());
            }
            return mapNames;
        }

        public static Map GetMapFromName(string name)
        {
            return this.list.Find(e => e.GetName() == name);
        }
    }
}