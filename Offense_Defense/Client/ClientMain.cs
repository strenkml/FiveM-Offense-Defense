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
        const int timeResetButtonPressedThreshold = 100;

        bool configMenuShown = false;
        bool isConfigLocked = true;

        List<Vector3> checkpoints = new List<Vector3>();

        bool recentReset = false;
        bool collision = true;
        int timeSinceReset = 0;
        const int resetNoCollisionTime = 100;

        /* -------------------------------------------------------------------------- */
        /*                                 Constructor                                */
        /* -------------------------------------------------------------------------- */
        public ClientMain()
        {
            Debug.WriteLine("Hi from OffenseDefense.Client!");

            // TODO: Move to startGame function
            offDefGame = new OffDefGame();

            // Commands
            // User Commands
            API.RegisterCommand("showConfig", new Action<int, List<object>, string>(ShowMenu), false);
            API.RegisterCommand("hideConfig", new Action<int, List<object>, string>(HideMenu), false);
            API.RegisterCommand("join", new Action<int, List<object>, string>(JoinTeam), false);
            API.RegisterCommand("leave", new Action<int, List<object>, string>(LeaveTeam), false);
            API.RegisterCommand("runner", new Action<int, List<object>, string>(JoinRunner), false);

            // TODO: REMOVE ME
            API.RegisterCommand("delAll", new Action<int, List<object>, string>(RemoveAllCars), false);
            API.RegisterCommand("del", new Action<int, List<object>, string>(RemoveMyCar), false);
            API.RegisterCommand("car", new Action(Car), false);

            // Event Handlers
            EventHandlers.Add("OffDef:UpdateTeams", new Action<dynamic>(UpdateTeams));
            EventHandlers.Add("OffDef:StartGame", new Action<string>(StartGame));
            EventHandlers.Add("OffDef:StartConfig", new Action(StartConfig));
            EventHandlers.Add("OffDef:SetConfigLock", new Action<bool>(SetConfigLock));
            EventHandlers.Add("OffDef:ShowConfig", new Action(ShowMenu));
            EventHandlers.Add("OffDef:HideConfig", new Action(HideMenu));
            EventHandlers.Add("OffDef:ShowGameMenu", new Action(ShowStartMenu));
            EventHandlers.Add("OffDef:SendError", new Action<string>(SendError));
            EventHandlers.Add("OffDef:SetTeamSpawn", new Action<string, Vector3, float>(SetTeamSpawn));



            // NUI Callbacks
            API.RegisterNuiCallbackType("startGame");
            EventHandlers["__cfx_nui:startGame"] += new Action<IDictionary<string, object>, CallbackDelegate>((data, cb) =>
            {
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

        private void RemoveAllCars(int source, List<object> args, string raw)
        {
            Vehicle[] cars = World.GetAllVehicles();
            foreach (Vehicle car in cars)
            {
                car.Delete();
            }
        }

        private void RemoveMyCar(int source, List<object> args, string raw)
        {
            offDefGame.DestroyCar();
        }

        private async void Car()
        {
            Vehicle car = await World.CreateVehicle(VehicleHash.Voodoo, Game.Player.Character.Position);
        }

        /* -------------------------------------------------------------------------- */
        /*                                Event Methods                               */
        /* -------------------------------------------------------------------------- */
        private void UpdateTeams(dynamic teams)
        {
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
            Vector3 runnerSpawnLoc;
            float runnerSpawnHeading;
            Vector3 blockerSpawnLoc;
            float blockerSpawnHeading;
            string runnerCar;
            string blockerCar;
            List<Shared.MapMarker> checkpointLocs;

            Util.GetPlayerDetails(jsonDetails, out role, out color, out runnerSpawnLoc, out runnerSpawnHeading, out blockerSpawnLoc, out blockerSpawnHeading, out checkpointLocs, out runnerCar, out blockerCar);

            this.collision = true;

            offDefGame.SetRole(role);
            offDefGame.SetTeamColor(color);
            offDefGame.SetCarTypes(runnerCar, blockerCar);
            offDefGame.SetSpawn(runnerSpawnLoc, runnerSpawnHeading, blockerSpawnLoc, blockerSpawnHeading);
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

        private void SetTeamSpawn(string color, Vector3 pos, float heading)
        {
            if (color == offDefGame.GetTeamColor())
            {
                Debug.WriteLine("setting the new spawn");
                offDefGame.SetSpawn(pos, heading, pos, heading);
            }
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
                    recentReset = false;
                }
                else
                {
                    if (!recentReset)
                    {
                        recentReset = true;
                        this.timeSinceReset = 0;
                        this.collision = false;
                        offDefGame.SetCarCollision(false);

                        offDefGame.RespawnPlayer();
                        Util.SendChatMsg("Respawned!");
                    }
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

            if (offDefGame.gameActive)
            {
                offDefGame.DisableControls();

                // Remove NPCs
                API.SetPedDensityMultiplierThisFrame(0.0f);
                API.SetScenarioPedDensityMultiplierThisFrame(0.0f, 0.0f);
                API.SetVehicleDensityMultiplierThisFrame(0.0f);
                API.SetRandomVehicleDensityMultiplierThisFrame(0.0f);
                API.SetParkedVehicleDensityMultiplierThisFrame(0.0f);

                if (!offDefGame.checkActive)
                {
                    offDefGame.CheckCheckpoints();
                }

                CheckRestartKeys();
            }

            if (this.collision == false)
            {
                if (this.timeSinceReset < resetNoCollisionTime)
                {
                    this.timeSinceReset++;
                }
                else
                {
                    this.timeSinceReset = 0;
                    collision = true;
                    offDefGame.SetCarCollision(true);
                }

            }


            return Task.FromResult(0);
        }
    }
}
