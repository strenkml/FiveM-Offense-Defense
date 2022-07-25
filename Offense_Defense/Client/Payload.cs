namespace OffenseDefense.Client
{
    class Payload
    {
        // Team Config
        public bool configEnable { get; set; }
        public object configPayload { get; set; }
        public bool configLock { get; set; }

        // Game Scoreboard
        public bool scoreboardEnable { get; set; }
        public object scoreboardPayload { get; set; }
        public int scoreboardNeededPoints { get; set; }

        // Create Game Menu
        public bool createGameEnable { get; set; }

        public Payload()
        {
            this.configEnable = false;
            this.configPayload = null;
            this.configLock = false;

            this.scoreboardEnable = false;
            this.scoreboardPayload = null;
            this.scoreboardNeededPoints = 0;

            this.createGameEnable = false;
        }
    }
}