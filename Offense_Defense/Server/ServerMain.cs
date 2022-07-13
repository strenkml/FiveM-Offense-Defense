using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace OffenseDefense.Server
{
    public class ServerMain : BaseScript
    {
        readonly string[] teamColors = { "blue", "red", "green", "orange", "yellow", "pink", "purple", "white" };
        Dictionary<string, Team> teams = new Dictionary<string, Team>();

        List<Player> players = new List<Player>();

        Dictionary<string, bool> playersReady;

        public ServerMain()
        {
            Debug.WriteLine("Hi from OffenseDefense.Server!");

            // Create the teams
            foreach (string color in teamColors)
            {
                teams.Add(color, new Team(color));
            }

            // Event Handlers
            EventHandlers.Add("OffDef:StartConfig", new Action(StartConfig));
            EventHandlers.Add("OffDef:AddPlayer", new Action<string, string>(AddPlayer));
            EventHandlers.Add("OffDef:RemovePlayer", new Action<string>(RemovePlayer));
            EventHandlers.Add("OffDef:SetRunner", new Action<string>(SetRunner));
            EventHandlers.Add("OffDef:SetTeamSpawnLocation", new Action<string, Vector3, float>(SetTeamSpawn));
            EventHandlers.Add("OffDef:SetConfigLock", new Action<bool>(SetConfigLock));
            EventHandlers.Add("OffDef:ClientReady", new Action<string>(SetClientReady));

            // General Handlers
            EventHandlers.Add("playerJoining", new Action<string, string>(OnPlayerJoiningServer));
        }

        /* -------------------------------------------------------------------------- */
        /*                            Event Handler Methods                           */
        /* -------------------------------------------------------------------------- */
        private void StartConfig()
        {
            foreach (Player p in Players)
            {
                players.Add(p);
                playersReady.Add(p.Name, false);
            }

            TriggerClientEvent("OffDef:SetConfigLock", false);
        }

        private void AddPlayer(string color, string name)
        {
            string otherTeam = Util.IsPlayerInOtherTeam(color, name, teams);
            if (otherTeam != "")
            {
                Team oldTeam = this.teams[otherTeam];
                oldTeam.RemovePlayer(name);
            }
            Team team = this.teams[color];
            team.AddPlayer(name);
            TriggerClientEvent("OffDef:UpdateTeams", this.teams);
        }

        private void RemovePlayer(string name)
        {
            string color = Util.GetPlayerTeam(name, teams);
            if (color != "")
            {
                Team team = this.teams[color];
                team.RemovePlayer(name);
                TriggerClientEvent("OffDef:UpdateTeams", this.teams);
            }
        }

        private void SetRunner(string name)
        {
            string color = Util.GetPlayerTeam(name, teams);
            if (color != "")
            {
                Team team = this.teams[color];
                bool result = team.SetRole(name, "Runner");
                if (!result)
                {
                    Debug.WriteLine($"ERROR: Failed to set player {name} to runner for {color} team");
                }
                TriggerClientEvent("OffDef:UpdateTeams", this.teams);
            }
        }

        private void SetTeamSpawn(string color, Vector3 newSpawn, float newHeading)
        {
            List<string> teamPlayers = this.teams[color].GetPlayers();
            foreach (string tp in teamPlayers)
            {
                Player p = players.Find(e => e.Name == tp);
                if (p != null)
                {
                    TriggerClientEvent(p, "OffDef:SetSpawn", newSpawn, newHeading);
                }
            }
        }

        private void SetConfigLock(bool newLock)
        {
            TriggerClientEvent("OffDef:SetConfigLock", newLock);
        }

        private void SetClientReady(string clientName)
        {
            playersReady[clientName] = true;
        }

        /* -------------------------------------------------------------------------- */
        /*                               General Events                               */
        /* -------------------------------------------------------------------------- */
        private void OnPlayerJoiningServer(string source, string oldID)
        {
            // TODO: Switch all instances of the user's name to use the user's handle            
        }


        [Command("hello_server")]
        public void HelloServer()
        {
            Debug.WriteLine("Sure, hello.");
        }

        /* -------------------------------------------------------------------------- */
        /*                               General Methods                              */
        /* -------------------------------------------------------------------------- */
        private bool IsAllPlayersReady()
        {
            foreach (KeyValuePair<string, bool> kp in playersReady)
            {
                if (!kp.Value)
                {
                    return false;
                }
            }
            return true;
        }

        /* -------------------------------------------------------------------------- */
        /*                                    Clock                                   */
        /* -------------------------------------------------------------------------- */
        [Tick]
        public Task OnTick()
        {
            if (IsAllPlayersReady() && players.Count > 0)
            {
                TriggerClientEvent("OffDef:AllTeamsReady", true);
            }
            return Task.FromResult(0);
        }

    }
}
