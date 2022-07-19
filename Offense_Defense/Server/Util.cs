using System.Collections.Generic;
using System.Linq;

namespace OffenseDefense.Server
{
    class Util
    {
        public static string IsPlayerInOtherTeam(string excludeColor, string name, Dictionary<string, Team> teams)
        {
            foreach (KeyValuePair<string, Team> entry in teams)
            {
                if (entry.Key != excludeColor)
                {
                    if (entry.Value.IsPlayer(name))
                    {
                        return entry.Key;
                    }
                }
            }
            return "";
        }

        public static string GetPlayerTeam(string name, Dictionary<string, Team> teams)
        {
            foreach (KeyValuePair<string, Team> entry in teams)
            {
                if (entry.Value.IsPlayer(name))
                {
                    return entry.Key;
                }
            }
            return "";
        }

        public static List<RankedScore> UpdateTeamPositions(Dictionary<string, Team> teams)
        {
            List<KeyValuePair<string, Team>> filtered = teams.ToList<KeyValuePair<string, Team>>().FindAll(e => e.Value.GetPlayers().Count != 0);

            List<RankedScore> rankings = new List<RankedScore>();

            if (filtered.Count > 0)
            {
                foreach (KeyValuePair<string, Team> kp in filtered)
                {
                    rankings.Add(new RankedScore(kp.Key, kp.Value.GetPoints()));
                }
            }

            return rankings.OrderByDescending(x => x.points).ToList<RankedScore>();
        }

        public static bool EveryTeamHasRunner(Dictionary<string, Team> teams)
        {
            foreach (KeyValuePair<string, Team> kp in teams)
            {
                if (kp.Value.runner == "")
                {
                    return false;
                }
            }
            return true;
        }
    }
}