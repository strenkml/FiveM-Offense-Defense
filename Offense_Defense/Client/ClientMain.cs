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
        bool recentReset = false;

        bool configMenuShown = false;
        bool isConfigLocked = true;

        List<Vector3> checkpoints = new List<Vector3>();

        bool collision = false;
        int noCollisionTime = -1;
        int noCollisionTimeThres = 400;

        int noCollisionColorTime = 0;
        const int noCollisionColorTimeThres = 10;
        bool noCollisionColorReal = true;
        TeamColor carColor;
        TeamColor lightCarColor;

        /* -------------------------------------------------------------------------- */
        /*                                 Constructor                                */
        /* -------------------------------------------------------------------------- */
        public ClientMain()
        {
            Debug.WriteLine("Hi from OffenseDefense.Client!");

            offDefGame = new OffDefGame();

            // Commands
            // User Commands
            API.RegisterCommand("showConfig", new Action<int, List<object>, string>(ShowMenu), false);
            API.RegisterCommand("hideConfig", new Action<int, List<object>, string>(HideMenu), false);
            API.RegisterCommand("join", new Action<int, List<object>, string>(JoinTeam), false);
            API.RegisterCommand("leave", new Action<int, List<object>, string>(LeaveTeam), false);
            API.RegisterCommand("runner", new Action<int, List<object>, string>(JoinRunner), false);
            API.RegisterCommand("modVersion", new Action<int, List<object>, string>(PrintVersion), false);

            // TODO: REMOVE ME
            API.RegisterCommand("delAll", new Action<int, List<object>, string>(RemoveAllCars), false);
            API.RegisterCommand("del", new Action<int, List<object>, string>(RemoveMyCar), false);
            API.RegisterCommand("car", new Action<int, List<object>, string>(Car), false);
            API.RegisterCommand("testSpawns", new Action(TestSpawns), false);
            API.RegisterCommand("tp", new Action(tpToMap), false);

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

            // Event Handlers for Game class
            EventHandlers.Add("OffDef:CountdownTimer", new Action<int>(offDefGame.SendCountdownTimer));
            EventHandlers.Add("OffDef:EndGame", new Action<string>(offDefGame.EndGame));
            EventHandlers.Add("OffDef:UpdateScoreboard", new Action<dynamic, int>(offDefGame.UpdateScoreboard));

            // TODO: Remove me
            EventHandlers.Add("OffDef:SpawnCarAtLoc", new Action<string, Vector3, float>(SpawnCarAtLoc));

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

            API.RegisterNuiCallbackType("closePanel");
            EventHandlers["__cfx_nui:closePanel"] += new Action<IDictionary<string, object>, CallbackDelegate>((data, cb) =>
            {
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

        private void PrintVersion(int source, List<object> args, string raw)
        {
            Util.SendChatMsg($"Mod Version: {Shared.Version.Major}.{Shared.Version.Minor}-{Shared.Version.FixVersion}", 255, 255, 0);
        }

        // TODO: Remove me
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
            Game.Player.Character.CurrentVehicle.Delete();
        }

        private async void Car(int source, List<object> args, string raw)
        {
            string carModel = args[0].ToString();
            if (Util.IsPossibleCar(carModel))
            {
                Vehicle car = await World.CreateVehicle(carModel, Game.Player.Character.Position);
            }
        }

        private void TestSpawns()
        {
            TriggerServerEvent("OffDef:testSpawns", Game.Player.Name);
        }

        private void tpToMap()
        {
            Game.Player.Character.Position = new Vector3(806.858f, 1280.028f, 360.348f);
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

            this.collision = false;
            this.noCollisionTime = -1;

            RemoveAllCars(0, null, "");

            this.carColor = TeamColors.GetTeamColorFromName(color);
            this.lightCarColor = TeamColors.GetLightTeamColorFromName(color);

            World.Weather = Weather.Clear;
            World.CurrentDayTime = TimeSpan.FromHours(12);

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

        private async void SpawnCarAtLoc(string type, Vector3 pos, float heading)
        {
            VehicleHash carType;
            if (type == "Runner")
            {
                carType = VehicleHash.Voodoo;
            }
            else
            {
                carType = VehicleHash.Insurgent2;
            }

            pos.Z += 15;
            Util.RequestModel(carType.ToString());
            Vehicle car = await World.CreateVehicle(carType, pos, heading);
            car.PlaceOnGround();
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

                        offDefGame.RespawnPlayer();

                        this.collision = false;
                        this.noCollisionTime = 0;
                        this.noCollisionTimeThres = 300;

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

            // After Client is ready
            if (offDefGame.GameActive)
            {
                offDefGame.DisableSettingsPerFrame();

                Vehicle veh = Game.Player.Character.LastVehicle;
                if (veh != null)
                {
                    List<Vehicle> allVeh = new List<Vehicle>(World.GetAllVehicles());

                    for (int i = 0; i < allVeh.Count; i++)
                    {
                        if (veh != allVeh[i])
                        {
                            if (this.collision)
                            {
                                allVeh[i].IsCollisionEnabled = true;
                                veh.IsCollisionEnabled = true;
                            }
                            else
                            {
                                allVeh[i].SetNoCollision(veh, true);
                            }
                        }
                    }
                }

                if (!this.collision && this.noCollisionTime != -1)
                {
                    if (this.noCollisionTime < noCollisionTimeThres)
                    {
                        this.noCollisionTime++;
                    }
                    else
                    {
                        this.noCollisionTime = 0;
                        this.collision = true;

                        Util.SendChatMsg("Collision Enabled!", 255, 255, 0);
                    }
                }
            }

            // Post countdown
            if (offDefGame.GameStarted)
            {
                if (this.noCollisionTime == -1)
                {
                    this.noCollisionTime = 0;
                    this.collision = false;
                }

                if (!offDefGame.checkActive)
                {
                    offDefGame.CheckCheckpoints();
                }

                CheckRestartKeys();

                if (!collision)
                {
                    if (noCollisionColorTime < noCollisionColorTimeThres)
                    {
                        noCollisionColorTime++;
                    }
                    else
                    {
                        noCollisionColorTime = 0;

                        if (noCollisionColorReal)
                        {
                            Util.SetCarColor(Game.Player.Character.CurrentVehicle, lightCarColor.r, lightCarColor.g, lightCarColor.b);
                        }
                        else
                        {
                            Util.SetCarColor(Game.Player.Character.CurrentVehicle, carColor.r, carColor.g, carColor.b);
                        }
                        noCollisionColorReal = !noCollisionColorReal;
                    }
                }
                else
                {
                    if (!noCollisionColorReal)
                    {
                        Util.SetCarColor(Game.Player.Character.CurrentVehicle, carColor.r, carColor.g, carColor.b);
                    }
                }

            }

            return Task.FromResult(0);
        }
    }
}
