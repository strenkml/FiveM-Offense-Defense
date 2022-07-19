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

        public static bool IsAllTeamsCompletedRace(Dictionary<string, Team> teams, int neededPoints)
        {
            foreach (KeyValuePair<string, Team> kp in teams)
            {
                if (!kp.Value.HasCompletedRace(neededPoints))
                {
                    return false;
                }
            }
            return true;
        }

        public static List<RankedScore> UpdateUncompleteTeamPositions(Dictionary<string, Team> teams)
        {
            List<KeyValuePair<string, Team>> filtered = teams.ToList<KeyValuePair<string, Team>>().FindAll(e => e.Value.GetPlayers().Count != 0 && !e.Value.completedRace);

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

        public static List<RankedScore> UpdateCompleteTeamPositions(Dictionary<string, Team> teams)
        {
            List<KeyValuePair<string, Team>> filtered = teams.ToList<KeyValuePair<string, Team>>().FindAll(e => e.Value.GetPlayers().Count != 0 && e.Value.completedRace);

            RankedScore[] rankings = new RankedScore[filtered.Count];
            if (filtered.Count > 0)
            {
                foreach (KeyValuePair<string, Team> kp in filtered)
                {
                    rankings[kp.Value.completedPosition - 1] = new RankedScore(kp.Key, kp.Value.GetPoints());
                }
            }

            return rankings.ToList<RankedScore>();
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