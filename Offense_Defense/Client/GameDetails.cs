using Newtonsoft.Json;
using CitizenFX.Core;
using System.Collections.Generic;

namespace OffenseDefense.Client
{
    class GameDetails
    {
        [JsonProperty("checkpoints")]
        public List<Vector3> checkpoints { get; set; }

        [JsonProperty("spawn")]
        public Vector3 spawn { get; set; }

        [JsonProperty("heading")]
        public float heading { get; set; }

        [JsonProperty("role")]
        public string role { get; set; }

        [JsonProperty("color")]
        public string color { get; set; }

        [JsonProperty("runnerCar")]
        public string runnerCar { get; set; }

        [JsonProperty("blockerCar")]
        public string blockerCar { get; set; }
    }
}