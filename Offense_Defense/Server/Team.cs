using System.Collections.Generic;

namespace OffenseDefense.Server
{
    class Team
    {
        private string color;
        private List<string> players;
        private string runner;
        private List<string> blockers;

        public Team(string color)
        {
            this.color = color;
            this.runner = "";
            this.blockers = new List<string>();
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

    }
}