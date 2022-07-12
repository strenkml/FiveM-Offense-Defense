using System.Collections.Generic;
using CitizenFX.Core.Native;

namespace OffenseDefense.Server {
    class Util {
        public static string IsPlayerInOtherTeam(string excludeColor, string name, dynamic teams)
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

        public static string GetPlayerTeam(string name, dynamic teams)
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
    }
}