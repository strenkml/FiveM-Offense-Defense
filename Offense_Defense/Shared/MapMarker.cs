using CitizenFX.Core;

namespace OffenseDefense.Shared
{
    class MapMarker
    {
        public Vector3 position { get; set; }
        public float heading { get; set; }

        public MapMarker(float x, float y, float z, float heading)
        {
            this.position = new Vector3(x, y, z - 1f);
            this.heading = heading;
        }
    }
}