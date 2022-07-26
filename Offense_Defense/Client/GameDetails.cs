using Newtonsoft.Json;
using CitizenFX.Core;
using System.Collections.Generic;

namespace OffenseDefense.Client
{
    class GameDetails
    {
        [JsonProperty("checkpoints")]
        public List<Shared.MapMarker> checkpoints { get; set; }

        [JsonProperty("runnerSpawn")]
        public Vector3 runnerSpawn { get; set; }

        [JsonProperty("runnerHeading")]
        public float runnerHeading { get; set; }

        [JsonProperty("blockerSpawn")]
        public Vector3 blockerSpawn { get; set; }

        [JsonProperty("blockerHeading")]
        public float blockerHeading { get; set; }

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