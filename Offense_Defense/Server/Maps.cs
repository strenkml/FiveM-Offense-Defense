using System.Collections.Generic;

namespace OffenseDefense.Server
{
    class Maps
    {
        private static Map map1 = new Map("Map 1",
        new Shared.MapMarker(677.147f, 1356.288f, 327.936f, 57.944f),
        new Shared.MapMarker(806.858f, 1280.028f, 360.348f, 270.380f),
        new List<Shared.MapMarker>() {
            new Shared.MapMarker(464.720f, 874.021f, 198.161f, 67.162f),
            new Shared.MapMarker(231.168f, 1376.892f, 239.621f, 23.198f),
            new Shared.MapMarker(317.858f, 1801.451f, 222.876f, 48.453f),
            new Shared.MapMarker(195.895f, 1897.547f, 173.423f, 140.996f),
            new Shared.MapMarker(45.525f, 2067.089f, 156.044f, 72.257f),
            new Shared.MapMarker(86.097f, 2292.944f, 101.865f, 286.909f),
            new Shared.MapMarker(91.049f, 2711.216f, 54.340f, 41.724f),
            new Shared.MapMarker(-186.817f, 2592.487f, 61.297f, 178.707f),
            new Shared.MapMarker(-230.841f, 2079.207f, 138.382f, 172.804f),
            new Shared.MapMarker(-488.671f, 1994.001f, 207.127f, 87.658f),
            new Shared.MapMarker(-780.491f, 1657.666f, 201.815f, 198.144f),
            new Shared.MapMarker(-460.499f, 1364.085f, 301.620f, 176.322f),
            new Shared.MapMarker(-409.192f, 1183.834f, 325.545f, 263.682f),
            });

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