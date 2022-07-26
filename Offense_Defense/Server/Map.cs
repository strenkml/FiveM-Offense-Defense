using System.Collections.Generic;
using CitizenFX.Core;

namespace OffenseDefense.Server
{
    class Map
    {
        private string name;
        private Dictionary<string, Shared.MapMarker> runnerStarting;
        private Dictionary<string, Shared.MapMarker> blockerStarting;
        private List<Shared.MapMarker> checkpoints;

        private int totalCheckpoints;

        public Map(string name, Dictionary<string, Shared.MapMarker> runnerStarting, Dictionary<string, Shared.MapMarker> blockerStarting, List<Shared.MapMarker> checkpoints)
        {
            this.name = name;
            this.runnerStarting = runnerStarting;
            this.blockerStarting = blockerStarting;
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

        public List<Shared.MapMarker> GetCheckpoints()
        {
            return this.checkpoints;
        }

        public Vector3 GetRunnerStartingSpawn(string team)
        {
            return this.runnerStarting[team].position;
        }

        public float GetRunnerStartingHeading(string team)
        {
            return this.runnerStarting[team].heading;
        }

        public Vector3 GetBlockerStartingSpawn(string team)
        {
            return this.blockerStarting[team].position;
        }

        public float GetBlockerStartingHeading(string team)
        {
            return this.blockerStarting[team].heading;
        }

        public Dictionary<string, Shared.MapMarker> GetAllRunnerStarting()
        {
            return this.runnerStarting;
        }

        public Dictionary<string, Shared.MapMarker> GetAllBlockerStarting()
        {
            return this.blockerStarting;
        }
    }
}