using System.Collections.Generic;
using CitizenFX.Core;

namespace OffenseDefense.Server
{
    class Map
    {
        private string name;

        private int totalCheckpoints;
        private List<Vector3> checkpoints;
        private Vector3 runnerSpawnLoc;
        private float runnerSpawnHeading;
        private Vector3 blockerSpawnLoc;
        private float blockerSpawnHeading;

        public Map(string name, Vector3 runnerSpawn, float runnerSpawnHeading, Vector3 blockerSpawn, float blockerSpawnHeading, List<Vector3> checkpoints)
        {
            this.name = name;
            this.runnerSpawnLoc = runnerSpawn;
            this.runnerSpawnHeading = runnerSpawnHeading;
            this.blockerSpawnLoc = blockerSpawn;
            this.blockerSpawnHeading = blockerSpawnHeading;
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

        public Vector3 GetRunnerSpawn()
        {
            return this.runnerSpawnLoc;
        }

        public float GetRunnerSpawnHeading()
        {
            return this.runnerSpawnHeading;
        }

        public Vector3 GetBlockerSpawn()
        {
            return this.blockerSpawnLoc;
        }

        public float GetBlockerSpawnHeading()
        {
            return this.blockerSpawnHeading;
        }
    }
}