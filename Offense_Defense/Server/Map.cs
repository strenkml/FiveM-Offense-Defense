using System.Collections.Generic;

namespace OffenseDefense.Server
{
    class Map
    {
        private string name;
        private string fileName;

        private int totalCheckpoints;
        private List<Vector3> checkpoints;
        private Vector3 spawnLoc;
        private float spawnHeading;

        public Map(string name, Vector3 spawn, float spawnHeading, List<Vector3> checkpoints)
        {
            this.name = name;
            this.spawnLoc = spawn;
            this.spawnHeading = spawnHeading;
            this.checkpoints = checkpoints;
            this.totalCheckpoints = checkpoints.Count;
        }

        public int GetTotalCheckpoints()
        {
            return this.totalCheckpoints;
        }

        public string GetName()
        {
            return this.name;
        }

        public List<Vector3> GetCheckpoints()
        {
            return this.checkpoints;
        }

        public Vector3 GetSpawn()
        {
            return this.spawnLoc;
        }

        public float GetSpawnHeading()
        {
            return this.spawnHeading;
        }
    }
}