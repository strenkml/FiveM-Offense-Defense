using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace OffenseDefense.Server
{
    public class ServerMain : BaseScript
    {
        readonly string[] teamColors = { "blue", "red", "green", "orange", "yellow", "pink", "purple", "white" };
        Dictionary<string, Team> teams = new Dictionary<string, Team>();
        List<RankedScore> rankedTeams = new List<RankedScore>();

        List<Player> players = new List<Player>();

        Dictionary<string, bool> playersReady = new Dictionary<string, bool>();
        bool oldAllPlayersReady = false;

        // Game Countdown
        const int countdownStart = 5;
        const int timePerCountdown = 35;
        int currrentCountdownTime = 0;
        bool countdownActive = false;
        int countdownCount = countdownStart;

        // Game info
        Map currentMap;

        public ServerMain()
        {
            Debug.WriteLine("Hello from the OffenseDefense Server!");

            // Create the teams
            CreateTeams();

            // Server Commands
            API.RegisterCommand("lockConfig", new Action<int, List<object>, string>(LockConfig), false);
            API.RegisterCommand("unlockConfig", new Action<int, List<object>, string>(UnlockConfig), false);
            API.RegisterCommand("startConfig", new Action<int, List<object>, string>(StartConfig), false);
            API.RegisterCommand("startGame", new Action<int, List<object>, string>(ShowGameMenu), false);
            API.RegisterCommand("resetTeams", new Action<int, List<object>, string>(ResetTeams), false);

            // Event Handlers
            EventHandlers.Add("OffDef:AddPlayer", new Action<string, string>(AddPlayer));
            EventHandlers.Add("OffDef:RemovePlayer", new Action<string>(RemovePlayer));
            EventHandlers.Add("OffDef:SetRunner", new Action<string>(SetRunner));
            EventHandlers.Add("OffDef:ClientReady", new Action<string>(SetClientReady));
            EventHandlers.Add("OffDef:AddTeamPoint", new Action<string>(AddTeamPoint));
            EventHandlers.Add("OffDef:StartingGameFromNUI", new Action<string, string, string>(StartGame));

            // General Handlers
            EventHandlers.Add("playerJoining", new Action<string, string>(OnPlayerJoiningServer));
            EventHandlers.Add("playerLeaving", new Action<string, string>(OnPlayerLeavingServer));
        }

        /* -------------------------------------------------------------------------- */
        /*                               Server Commands                              */
        /* -------------------------------------------------------------------------- */
        private void StartConfig(int source, List<object> args, string raw)
        {
            SetPlayers();
            SetReadyPlayer();

            TriggerClientEvent("OffDef:StartConfig");
            TriggerClientEvent("OffDef:SetConfigLock", false);
        }

        private void ShowGameMenu(int source, List<object> args, string raw)
        {
            Player p = this.players.Find(e => e.Handle == source.ToString());
            if (Util.EveryTeamHasRunner(this.teams))
            {
                TriggerClientEvent("OffDef:SetConfigLock", true);
                TriggerClientEvent("OffDef:HideConfig");

                TriggerClientEvent(p, "OffDef:ShowGameMenu");
            }
            else
            {
                TriggerClientEvent(p, "OffDef:SendError", "Not all teams have runners!");
            }
        }

        private void LockConfig(int source, List<object> args, string raw)
        {
            TriggerClientEvent("OffDef:SetConfigLock", true);
        }

        private void UnlockConfig(int source, List<object> args, string raw)
        {
            TriggerClientEvent("OffDef:SetConfigLock", false);
        }

        private void ResetTeams(int source, List<object> args, string raw)
        {
            CreateTeams();
            TriggerClientEvent("OffDef:UpdateTeams", this.teams);

        }

        /* -------------------------------------------------------------------------- */
        /*                            Event Handler Methods                           */
        /* -------------------------------------------------------------------------- */
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
            TriggerClientEvent("OffDef:UpdateTeams", teams);
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

        private void SetClientReady(string clientName)
        {
            Debug.WriteLine($"Client: {clientName} is ready");
            playersReady[clientName] = true;
        }

        private void AddTeamPoint(string team)
        {
            Team t = this.teams[team];
            t.IncPoints();

            Debug.WriteLine($"Team {team} got a point. Current points {t.GetPoints()}");

            if (t.HasCompletedRace(currentMap.GetTotalCheckpoints()))
            {
                Debug.WriteLine($"Winner Winner Chicken Diner: {team}");
                EndGame(team);
            }
            this.rankedTeams = Util.UpdateTeamPositions(this.teams);
            TriggerClientEvent("OffDef:UpdateScoreboard", this.rankedTeams, currentMap.GetTotalCheckpoints());
            TriggerClientEvent("OffDef:SetTeamSpawn", team, currentMap.GetCheckpoints()[t.GetPoints() - 1].position, currentMap.GetCheckpoints()[t.GetPoints() - 1].heading);
        }

        private void StartGame(string mapName, string runnerCar, string blockerCar)
        {
            currentMap = Maps.GetMapFromName(mapName);

            foreach (KeyValuePair<string, Team> kp in this.teams)
            {
                if (kp.Value.GetPlayers().Count > 0)
                {
                    Player runnerPlayer = this.players.Find(e => e.Name == kp.Value.runner);

                    this.rankedTeams = Util.UpdateTeamPositions(this.teams);

                    Debug.WriteLine(string.Join(", ", this.rankedTeams));
                    TriggerClientEvent("OffDef:UpdateScoreboard", this.rankedTeams, currentMap.GetTotalCheckpoints());

                    SendStartGameToClient(runnerPlayer, kp.Value.color, "Runner", runnerCar, blockerCar);

                    foreach (string blocker in kp.Value.blockers)
                    {
                        Player blockerPlayer = this.players.Find(e => e.Name == blocker);
                        SendStartGameToClient(blockerPlayer, kp.Value.color, "Blocker", runnerCar, blockerCar);
                    }
                }
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                               General Events                               */
        /* -------------------------------------------------------------------------- */
        private void OnPlayerJoiningServer(string source, string oldID)
        {
            string playerName = API.GetPlayerName(source);
            if (players.Find(e => e.Name == playerName) == null)
            {
                SetPlayers();
                SetReadyPlayer();
            }
        }

        private void OnPlayerLeavingServer(string source, string reason)
        {
            string playerName = API.GetPlayerName(source);
            Player thisUser = players.Find(e => e.Name == playerName);
            if (thisUser != null)
            {
                players.Remove(thisUser);
                playersReady.Remove(thisUser.Name);
            }
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

        private void SetPlayers()
        {
            players.Clear();
            foreach (Player p in Players)
            {
                players.Add(p);
            }
        }

        private void SetReadyPlayer()
        {
            playersReady.Clear();
            foreach (Player p in Players)
            {
                playersReady.Add(p.Name, false);
            }
        }

        private void Countdown()
        {
            if (this.countdownCount <= -1)
            {
                this.countdownCount = countdownStart;
                this.countdownActive = false;
            }
            else
            {
                TriggerClientEvent("OffDef:CountdownTimer", this.countdownCount);
                this.countdownCount--;
            }
        }

        private void SendStartGameToClient(Player player, string color, string role, string runnerCar, string blockerCar)
        {
            TriggerClientEvent(player, "OffDef:StartGame", JsonConvert.SerializeObject(new
            {
                checkpoints = currentMap.GetCheckpoints(),
                runnerSpawn = currentMap.GetRunnerStartingSpawn(color),
                runnerHeading = currentMap.GetRunnerStartingHeading(color),
                blockerSpawn = currentMap.GetBlockerStartingSpawn(color),
                blockerHeading = currentMap.GetBlockerStartingHeading(color),
                color = color,
                role = role,
                runnerCar = runnerCar,
                blockerCar = blockerCar
            }));
        }

        private void EndGame(string winningTeam)
        {
            TriggerClientEvent("OffDef:EndGame", winningTeam);
        }

        private void CreateTeams()
        {
            teams.Clear();
            foreach (string color in teamColors)
            {
                teams.Add(color, new Team(color));
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                                    Clock                                   */
        /* -------------------------------------------------------------------------- */
        [Tick]
        public Task OnTick()
        {
            // Check if all of the players are ready
            if (IsAllPlayersReady() && this.playersReady.Count > 0)
            {
                if (oldAllPlayersReady == false)
                {
                    this.countdownActive = true;
                }
                oldAllPlayersReady = true;
            }

            if (this.countdownActive)
            {
                if (this.currrentCountdownTime < timePerCountdown)
                {
                    this.currrentCountdownTime++;
                }
                else
                {
                    this.currrentCountdownTime = 0;
                    Countdown();
                }
            }
            return Task.FromResult(0);
        }

    }
}
