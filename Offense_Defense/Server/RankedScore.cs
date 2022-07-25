namespace OffenseDefense.Server
{
    class RankedScore
    {
        public string team { get; set; }
        public int points { get; set; }

        public RankedScore(string team, int points)
        {
            this.team = team;
            this.points = points;
        }

        public override string ToString()
        {
            return $"Team: {this.team}  Points: {this.points}";
        }
    }
}