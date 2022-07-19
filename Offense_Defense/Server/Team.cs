using System.Collections.Generic;
using CitizenFX.Core;

namespace OffenseDefense.Server
{
    class Team
    {
        public string color { get; set; }
        private List<string> players;
        public string runner { get; set; }
        public List<string> blockers { get; set; }
        public int points { get; set; }


        public Team(string color)
        {
            this.color = color;
            this.runner = "";
            this.blockers = new List<string>();
            this.players = new List<string>();
            this.points = 0;
        }

        public string GetColor()
        {
            return this.color;
        }

        public bool AddPlayer(string name)
        {
            if (!IsPlayer(name))
            {
                this.players.Add(name);

                this.blockers.Add(name);

                return true;
            }
            return false;
        }

        public bool RemovePlayer(string name)
        {
            if (IsPlayer(name))
            {
                this.players.Remove(name);

                if (IsBlocker(name))
                {
                    this.blockers.Remove(name);
                }

                if (IsRunner(name))
                {
                    this.runner = "";
                }
                return true;
            }
            return false;
        }

        public bool IsPlayer(string name)
        {
            return this.players.Contains(name);
        }

        public bool IsBlocker(string name)
        {
            return this.blockers.Contains(name);
        }

        public bool IsRunner(string name)
        {
            return this.runner == name;
        }

        public bool SetRole(string name, string role)
        {
            if (IsPlayer(name))
            {
                if (role == "Runner")
                {
                    if (!IsRunner(name))
                    {
                        if (this.runner != "")
                        {
                            SetRole(this.runner, "Blocker");
                        }
                        this.runner = name;
                    }

                    if (IsBlocker(name))
                    {
                        this.blockers.Remove(name);
                    }
                    return true;
                }
                else if (role == "Blocker")
                {
                    if (!IsBlocker(name))
                    {
                        this.blockers.Add(name);
                    }

                    if (IsRunner(name))
                    {
                        this.runner = "";
                    }
                    return true;
                }
            }
            return false;
        }

        public List<string> GetPlayers()
        {
            return this.players;
        }

        public void SetPoints(int newPoints)
        {
            this.points = newPoints;
        }

        public int GetPoints()
        {
            return this.points;
        }

        public void IncPoints()
        {
            this.points++;
        }

        public void DecPoints()
        {
            if (this.points - 1 < 0)
            {
                this.points = 0;
            }
            else
            {
                this.points--;
            }
        }

        public void ResetScoring()
        {
            this.points = 0;
        }

        public bool HasCompletedRace(int totalMapCheckpoints)
        {
            return this.points >= totalMapCheckpoints;
        }
    }
}