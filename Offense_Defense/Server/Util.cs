using System.Collections.Generic;

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

        public static Map GetMapWithName(string name)
        {
            return Maps.list.Find(e => e.getName() == name);
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

        public static void UpdateTeamPositions(Dictionary<string, Team> teams)
        {
            Dictionary<string, int> rankings = new Dictionary<string, int>();
            foreach (KeyValuePair<string, Team> kp in teams)
            {
                rankings.Add(kp.Key, kp.Value.GetPoints());
            }

            // TODO: Find a way to sort the dictionary
        }
    }
}