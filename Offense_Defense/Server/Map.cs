using System.Collections.Generic;
using CitizenFX.Core;

namespace OffenseDefense.Server
{
    class Map
    {
        private string name;
        // The back most left starting spawn, the rest of the spawns will be calculated
        private Shared.MapMarker initialRunnerStarting;
        // The back most left starting spawn, the rest of the spawns will be calculated
        private Shared.MapMarker initialBlockerStarting;
        private List<Shared.MapMarker> checkpoints;

        private int totalCheckpoints;

        private List<Shared.MapMarker> runnerStarting;
        private List<Shared.MapMarker> blockerStarting;

        private const float DISTANCE_BETWEEN_CARS = 5f;

        public Map(string name, Shared.MapMarker initialRunnerStarting, Shared.MapMarker initialBlockerStarting, List<Shared.MapMarker> checkpoints)
        {
            this.name = name;
            this.initialRunnerStarting = initialRunnerStarting;
            this.initialBlockerStarting = initialBlockerStarting;
            this.checkpoints = checkpoints;

            this.totalCheckpoints = checkpoints.Count;

            this.runnerStarting = CreateStartingMarkers(this.initialRunnerStarting);
            this.blockerStarting = CreateStartingMarkers(this.initialBlockerStarting);
        }

        public string GetName()
        {
            return this.name;
        }

        public List<Shared.MapMarker> GetCheckpoints()
        {
            return this.checkpoints;
        }

        public int GetTotalCheckpoints()
        {
            return this.totalCheckpoints;
        }

        private List<Shared.MapMarker> CreateStartingMarkers(Shared.MapMarker startingMarker) {
            List<Shared.MapMarker> list = new List<Shared.MapMarker>();

            // Create the backrow of spawns
            list.Add(startingMarker);
            Shared.MapMarker startingPoint = startingMarker;
            for (int i = 0; i < 3; i++) {
                startingPoint = Shared.Coords.GetRight(startingPoint, 5);
                list.Add(startingPoint);
            }

            // Create the front rightmost of spawns
            startingPoint = Shared.Coords.GetFront(startingPoint, 5);
            list.Add(startingPoint);

            for (int i = 0; i < 3; i++) {
                startingPoint = Shared.Coords.GetLeft(startingPoint, 5);
                list.Add(startingPoint);
            }

            // Index 0 = Back left most
            // Index 7 = Front left most
            return list;
        }

        public List<Shared.MapMarker> GetRunnerStartingMarkers() {
            return this.runnerStarting;
        }

        public List<Shared.MapMarker> GetBlockerStartingMarkers() {
            return this.blockerStarting;
        }

        public Shared.MapMarker GetRunnerStartingPosition(int index) {
            return this.runnerStarting[index].position;
        }

        public Shared.MapMarker GetRunnerStartingHeading(int index) {
            return this.runnerStarting[index].heading;
        }

        public Shared.MapMarker GetBlockerStartingPosition(int index) {
            return this.blockerStarting[index].position;
        }

        public Shared.MapMarker GetBlockerStartingHeading(int index) {
            return this.blockerStarting[index].heading;
        }
    }
}