using System.Collections.Generic;

namespace OffenseDefense.Client
{

    class Team
    {
        private string Color;
        private string Runner;
        private List<string> Blockers;
        public Team(string color)
        {
            this.Color = color;
            this.Runner = "";
            this.Blockers = new List<string>();
        }

        public void SetColor(string newColor)
        {
            this.Color = newColor;
        }

        public void SetRunner(string newRunner)
        {
            this.Runner = newRunner;
        }

        public void SetBlockers(List<string> newBlockers)
        {
            this.Blockers = newBlockers;
        }

        public string GetColor()
        {
            return this.Color;
        }

        public string GetRunner()
        {
            return this.Runner;
        }

        public List<string> GetBlockers()
        {
            return this.Blockers;
        }
    }
}