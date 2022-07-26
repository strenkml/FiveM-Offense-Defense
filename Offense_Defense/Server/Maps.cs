using System.Collections.Generic;

namespace OffenseDefense.Server
{
    class Maps
    {
        private static Map map1 = new Map("Map 1",
        new Dictionary<string, Shared.MapMarker>(){
            {"blue", new Shared.MapMarker(673.612f, 1370.508f, 326.657f, 59.840f)},
            {"red", new Shared.MapMarker(672.010f, 1367.657f, 326.685f, 55.887f)},
            {"green", new Shared.MapMarker(671.131f, 1364.928f, 326.762f, 58.856f)},
            {"orange", new Shared.MapMarker(667.790f, 1361.412f, 326.537f, 58.249f)},
            {"yellow", new Shared.MapMarker(677.296f, 1355.273f, 328.010f, 59.653f)},
            {"pink", new Shared.MapMarker(679.397f, 1359.809f, 328.061f, 62.597f)},
            {"purple", new Shared.MapMarker(680.611f, 1362.989f, 328.015f, 61.628f)},
            {"white", new Shared.MapMarker(685.165f, 1364.537f, 328.461f, 62.805f)},
        },
        new Dictionary<string, Shared.MapMarker>(){
            {"blue", new Shared.MapMarker(806.824f, 1280.178f, 360.346f, 261.717f)},
            {"red", new Shared.MapMarker(806.599f, 1276.452f, 360.363f, 263.713f)},
            {"green", new Shared.MapMarker(806.271f, 1274.173f, 360.353f, 267.799f)},
            {"orange", new Shared.MapMarker(806.549f, 1270.811f, 360.360f, 268.510f)},
            {"yellow", new Shared.MapMarker(814.169f, 1270.618f, 360.410f, 268.529f)},
            {"pink", new Shared.MapMarker(814.366f, 1274.085f, 360.431f, 271.962f)},
            {"purple", new Shared.MapMarker(813.981f, 1276.925f, 360.426f, 269.585f)},
            {"white", new Shared.MapMarker(816.302f, 1280.215f, 360.392f, 269.322f)},
        },
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