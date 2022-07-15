using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace OffenseDefense.Client
{
    public class ClientMain : BaseScript
    {
        /* -------------------------------------------------------------------------- */
        /*                                  Variables                                 */
        /* -------------------------------------------------------------------------- */
        OffDefGame offDefGame;
        readonly string[] teamColors = { "blue", "red", "green", "orange", "yellow", "pink", "purple", "white" };
        dynamic teams;

        // Timers
        const int menuUpdateThreshold = 2000;
        int menuUpdateTime = 0;

        int timeResetButtonPressed = 0;
        const int timeResetButtonPressedThreshold = 3000;

        bool configMenuShown = false;
        bool isConfigLocked = true;

        /* -------------------------------------------------------------------------- */
        /*                                 Constructor                                */
        /* -------------------------------------------------------------------------- */
        public ClientMain()
        {
            Debug.WriteLine("Hi from OffenseDefense.Client!");

            offDefGame = new OffDefGame();

            // Commands
            // TODO: Prepend all of the commands with od
            // User Commands
            API.RegisterCommand("showConfig", new Action(ShowMenu), false);
            API.RegisterCommand("hideConfig", new Action(HideMenu), false);
            API.RegisterCommand("joinTeam", new Action<string>(JoinTeam), false);
            API.RegisterCommand("leaveTeam", new Action(LeaveTeam), false);
            API.RegisterCommand("setRunner", new Action(JoinRunner), false);

            // Global Commands
            API.RegisterCommand("del", new Action(DeleteCars), false);
            API.RegisterCommand("startConfig", new Action(TriggerStartConfig), false);
            API.RegisterCommand("lockConfig", new Action(LockConfig), false);
            API.RegisterCommand("unlockConfig", new Action(UnlockConfig), false);
            API.RegisterCommand("startGame", new Action(TriggerStartGame), false);

            // Event Handlers
            EventHandlers.Add("OffDef:UpdateTeams", new Action<dynamic>(UpdateTeams));
            EventHandlers.Add("OffDef:StartGame", new Action<Dictionary<int, dynamic>>(StartGame));
            EventHandlers.Add("OffDef:StartConfig", new Action(StartConfig));
            EventHandlers.Add("OffDef:SetSpawn", new Action<Vector3, float>(SetSpawn));
            EventHandlers.Add("OffDef:SetConfigLock", new Action<bool>(SetConfigLock));
            EventHandlers.Add("OffDef:ShowConfig", new Action(ShowMenu));

        }

        /* -------------------------------------------------------------------------- */
        /*                            User Command Methods                            */
        /* -------------------------------------------------------------------------- */
        private void ShowMenu()
        {
            Util.SendNuiMessage(new { teamConfig = true, lockTeamConfig = isConfigLocked });
            API.SetNuiFocus(false, false);
            this.configMenuShown = true;
        }

        private void HideMenu()
        {
            Util.SendNuiMessage(new { teamConfig = false, lockTeamConfig = isConfigLocked });
            API.SetNuiFocus(false, false);
            this.configMenuShown = false;
        }

        private void JoinTeam(string teamColor)
        {
            Debug.WriteLine($"Config lock: {isConfigLocked}");
            if (!isConfigLocked)
            {
                teamColor = teamColor.ToLower();

                Debug.WriteLine("Checking is color");
                // Colors.PrintColors();
                if (Colors.IsColor(teamColor))
                {
                    string playerName = Game.Player.Name;

                    Debug.WriteLine("Joining team");

                    TriggerServerEvent("OffDef:AddPlayer", teamColor, playerName);
                }
            }
        }

        private void LeaveTeam()
        {
            if (!isConfigLocked)
            {
                string playerName = Game.Player.Name;

                TriggerServerEvent("OffDef:RemovePlayer", playerName);
            }
        }

        private void JoinRunner()
        {
            if (!isConfigLocked)
            {
                string playerName = Game.Player.Name;

                TriggerServerEvent("OffDef:SetRunner", playerName);
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                           Global Commands Methods                          */
        /* -------------------------------------------------------------------------- */
        private void DeleteCars()
        {
            Vehicle[] cars = World.GetAllVehicles();
            foreach (Vehicle car in cars)
            {
                car.Delete();
            }
        }

        private void LockConfig()
        {
            TriggerServerEvent("OffDef:SetConfigLock", true);
        }

        private void UnlockConfig()
        {
            TriggerServerEvent("OffDef:SetConfigLock", false);
        }

        private void TriggerStartConfig()
        {
            TriggerServerEvent("OffDef:StartConfig");
        }

        private void TriggerStartGame()
        {
            TriggerServerEvent("OffDef:StartGame");
        }


        /* -------------------------------------------------------------------------- */
        /*                                Event Methods                               */
        /* -------------------------------------------------------------------------- */
        private void UpdateTeams(dynamic teams)
        {
            Debug.WriteLine("Updating teams");
            this.teams = teams;
            UpdateMenu();
        }

        private void StartGame(Dictionary<int, dynamic> playerDetails)
        {
            string role;
            string color;
            Util.GetPlayerDetails(playerDetails, out role, out color);

            offDefGame.SetRole(role);
            offDefGame.SetTeamColor(color);
            offDefGame.StartGame();
        }

        private void SetSpawn(Vector3 newSpawn, float heading)
        {
            offDefGame.SetSpawn(newSpawn, heading);
        }

        private void StartConfig()
        {
            ShowMenu();
        }

        private void SetConfigLock(bool newLock)
        {
            this.isConfigLocked = newLock;
            if (newLock)
            {
                Util.SendNuiMessage(new { teamConfig = true, lockTeamConfig = true });
            }
            else
            {
                Util.SendNuiMessage(new { teamConfig = true, lockTeamConfig = false });
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                                 NUI Methods                                */
        /* -------------------------------------------------------------------------- */
        private void UpdateMenu()
        {
            // TODO: UPDATE TO NEW VALUE
            Util.SendNuiMessage(new { teams = teams });
        }

        /* -------------------------------------------------------------------------- */
        /*                            Button Press Methods                            */
        /* -------------------------------------------------------------------------- */
        private void CheckRestartKeys()
        {
            int f_key = 23;

            bool pressed = Util.IsButtonPressed(f_key);
            if (pressed && timeResetButtonPressed < timeResetButtonPressedThreshold)
            {
                timeResetButtonPressed++;
            }
            else if (!pressed)
            {
                timeResetButtonPressed = 0;
            }
            else
            {
                offDefGame.RespawnPlayer();
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                                    Clock                                   */
        /* -------------------------------------------------------------------------- */
        [Tick]
        public Task OnTick()
        {
            if (this.configMenuShown)
            {
                if (menuUpdateTime >= menuUpdateThreshold)
                {
                    menuUpdateTime = 0;
                    UpdateMenu();
                }
                else
                {
                    menuUpdateTime++;
                }
            }


            return Task.FromResult(0);
        }
    }
}
