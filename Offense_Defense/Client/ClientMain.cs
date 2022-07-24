using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

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
        const int timeResetButtonPressedThreshold = 300;

        bool configMenuShown = false;
        bool isConfigLocked = true;

        List<Vector3> checkpoints = new List<Vector3>();

        /* -------------------------------------------------------------------------- */
        /*                                 Constructor                                */
        /* -------------------------------------------------------------------------- */
        public ClientMain()
        {
            Debug.WriteLine("Hi from OffenseDefense.Client!");

            // TODO: Move to startGame function
            offDefGame = new OffDefGame();

            // Commands
            // TODO: Prepend all of the commands with od
            // User Commands
            API.RegisterCommand("showConfig", new Action<int, List<object>, string>(ShowMenu), false);
            API.RegisterCommand("hideConfig", new Action<int, List<object>, string>(HideMenu), false);
            API.RegisterCommand("j", new Action<int, List<object>, string>(JoinTeam), false);
            API.RegisterCommand("leaveTeam", new Action<int, List<object>, string>(LeaveTeam), false);
            API.RegisterCommand("r", new Action<int, List<object>, string>(JoinRunner), false);

            // TODO: REMOVE ME
            API.RegisterCommand("s", new Action<int, List<object>, string>(SetCarSpawn), false);
            API.RegisterCommand("del", new Action<int, List<object>, string>(RemoveAllCars), false);

            // Event Handlers
            EventHandlers.Add("OffDef:UpdateTeams", new Action<dynamic>(UpdateTeams));
            EventHandlers.Add("OffDef:StartGame", new Action<string>(StartGame));
            EventHandlers.Add("OffDef:StartConfig", new Action(StartConfig));
            EventHandlers.Add("OffDef:SetConfigLock", new Action<bool>(SetConfigLock));
            EventHandlers.Add("OffDef:ShowConfig", new Action(ShowMenu));
            EventHandlers.Add("OffDef:HideConfig", new Action(HideMenu));
            EventHandlers.Add("OffDef:ShowGameMenu", new Action(ShowStartMenu));
            EventHandlers.Add("OffDef:SendError", new Action<string>(SendError));


            // NUI Callbacks
            API.RegisterNuiCallbackType("startGame");
            EventHandlers["__cfx_nui:startGame"] += new Action<IDictionary<string, object>, CallbackDelegate>((data, cb) =>
            {
                Debug.WriteLine("Received NUI Callback");
                Debug.WriteLine(data["map"].ToString());
                Debug.WriteLine(data["runner"].ToString());
                Debug.WriteLine(data["blocker"].ToString());

                string map = data["map"].ToString();
                string runner = data["runner"].ToString();
                string blocker = data["blocker"].ToString();

                bool error = false;
                if (!Util.IsPossibleCar(runner) && runner != "")
                {
                    error = true;
                    Util.SendChatMsg("Runner Car invalid", 255, 0, 0);
                }

                if (!Util.IsPossibleCar(blocker) && blocker != "")
                {
                    error = true;
                    Util.SendChatMsg("Blocker Car invalid", 255, 0, 0);
                }

                if (!error)
                {
                    TriggerServerEvent("OffDef:StartingGameFromNUI", map, runner, blocker);
                }
                API.SetNuiFocus(false, false);
            });

            API.RegisterNuiCallbackType("error");
            EventHandlers["__cfx_nui:error"] += new Action<IDictionary<string, object>, CallbackDelegate>((data, cb) =>
            {
                Util.SendChatMsg(data["error"].ToString(), 255, 0, 0);
                API.SetNuiFocus(false, false);
            });

        }

        /* -------------------------------------------------------------------------- */
        /*                            User Command Methods                            */
        /* -------------------------------------------------------------------------- */
        private void ShowMenu(int source, List<object> args, string raw)
        {
            ShowMenu();
        }

        private void HideMenu(int source, List<object> args, string raw)
        {
            HideMenu();
        }

        private void JoinTeam(int source, List<object> args, string raw)
        {
            if (!isConfigLocked)
            {
                string teamColor = args[0].ToString().ToLower();

                // TeamColors.PrintColors();
                if (TeamColors.IsColor(teamColor))
                {
                    string playerName = Game.Player.Name;

                    TriggerServerEvent("OffDef:AddPlayer", teamColor, playerName);
                }
            }
        }

        private void LeaveTeam(int source, List<object> args, string raw)
        {
            if (!isConfigLocked)
            {
                string playerName = Game.Player.Name;

                TriggerServerEvent("OffDef:RemovePlayer", playerName);
            }
        }

        private void JoinRunner(int source, List<object> args, string raw)
        {
            if (!isConfigLocked)
            {
                string playerName = Game.Player.Name;

                TriggerServerEvent("OffDef:SetRunner", playerName);
            }
        }

        // TODO: DELETE ME
        private void SetCarSpawn(int source, List<object> args, string raw)
        {
            Ped p = Game.Player.Character;
            offDefGame.SetSpawn(p.Position, p.Heading);
        }

        private void RemoveAllCars(int source, List<object> args, string raw)
        {
            Vehicle[] cars = World.GetAllVehicles();
            foreach (Vehicle car in cars)
            {
                car.Delete();
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                                Event Methods                               */
        /* -------------------------------------------------------------------------- */
        private void UpdateTeams(dynamic teams)
        {
            Debug.WriteLine(JsonConvert.SerializeObject(teams));
            this.teams = teams;
            UpdateMenu();
        }

        private void StartConfig()
        {
            ShowMenu();
        }

        private void StartGame(string jsonDetails)
        {
            string role;
            string color;
            Vector3 spawnLoc;
            float spawnHeading;
            string runnerCar;
            string blockerCar;
            List<Vector3> checkpointLocs;

            Util.GetPlayerDetails(jsonDetails, out role, out color, out spawnLoc, out spawnHeading, out checkpointLocs, out runnerCar, out blockerCar);
            Debug.WriteLine(role);
            Debug.WriteLine(color);
            Debug.WriteLine(runnerCar);
            Debug.WriteLine(blockerCar);

            offDefGame.SetRole(role);
            offDefGame.SetTeamColor(color);
            offDefGame.SetCarTypes(runnerCar, blockerCar);
            // TODO: Uncomment below
            offDefGame.SetSpawn(spawnLoc, spawnHeading);
            offDefGame.SetCheckpoints(checkpointLocs);
            offDefGame.StartGame();
        }

        private void SetConfigLock(bool newLock)
        {
            this.isConfigLocked = newLock;
            if (newLock)
            {
                Payload payload = new Payload();
                payload.configEnable = true;
                payload.configLock = true;

                Util.SendNuiMessage(payload);
            }
            else
            {
                Payload payload = new Payload();
                payload.configEnable = true;
                payload.configLock = false;

                Util.SendNuiMessage(payload);
            }
        }

        private void ShowMenu()
        {
            Payload payload = new Payload();
            payload.configEnable = true;
            payload.configLock = isConfigLocked;

            Util.SendNuiMessage(payload);

            API.SetNuiFocus(false, false);
            this.configMenuShown = true;
        }

        private void HideMenu()
        {
            Payload payload = new Payload();
            payload.configEnable = false;
            payload.configLock = isConfigLocked;

            Util.SendNuiMessage(payload);

            API.SetNuiFocus(false, false);
            this.configMenuShown = false;
        }

        private void ShowStartMenu()
        {
            Payload payload = new Payload();
            payload.createGameEnable = true;

            Util.SendNuiMessage(payload);

            API.SetNuiFocus(true, true);
        }

        private void SendError(string error)
        {
            Util.SendChatMsg(error, 255, 0, 0);
        }

        /* -------------------------------------------------------------------------- */
        /*                                 NUI Methods                                */
        /* -------------------------------------------------------------------------- */
        private void UpdateMenu()
        {
            Payload payload = new Payload();
            payload.configEnable = true;
            payload.configPayload = teams;
            payload.configLock = isConfigLocked;

            Util.SendNuiMessage(payload);
        }

        /* -------------------------------------------------------------------------- */
        /*                            Button Press Methods                            */
        /* -------------------------------------------------------------------------- */
        private void CheckRestartKeys()
        {
            if (offDefGame != null)
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
