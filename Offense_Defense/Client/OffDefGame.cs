using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.UI;
using CitizenFX.Core.Native;

namespace OffenseDefense.Client
{
    class OffDefGame : BaseScript
    {
        /* -------------------------------------------------------------------------- */
        /*                                  Variables                                 */
        /* -------------------------------------------------------------------------- */
        private Vector3 spawn;
        private float heading;

        private List<Vector3> checkpoints;
        private bool[] completedCheckpoints;

        private Blip currentBlip;
        private int currentCheckpoint = -100;

        private string teamColor;
        private string role;

        private Vehicle myCar;

        private string runnerCar;
        private string blockerCar;

        // Countdown
        private const int timePerCountDown = 1000;
        private const int startingNumber = 5;
        private int currentCount = startingNumber;

        private bool gameActive = false;
        private bool gameOver = false;
        private string winningTeam = "";

        /* -------------------------------------------------------------------------- */
        /*                                 Constructor                                */
        /* -------------------------------------------------------------------------- */
        public OffDefGame()
        {
            Tick += onTick;

            // Events
            EventHandlers.Add("OffDef:CountdownTimer", new Action<int>(SendCountdownTimer));
            EventHandlers.Add("OffDef:SetSpawn", new Action<Vector3, float>(SetSpawn));
            EventHandlers.Add("OffDef:EndGame", new Action<string>(EndGame));
            EventHandlers.Add("OffDef:UpdateScoreboard", new Action<dynamic>(UpdateScoreboard));


            Game.Player.CanControlCharacter = true;
        }


        /* -------------------------------------------------------------------------- */
        /*                                    Clock                                   */
        /* -------------------------------------------------------------------------- */
        private async Task onTick()
        {
            if (gameActive)
            {
                DisableControls();

                // Remove NPCs
                API.SetPedDensityMultiplierThisFrame(0.0f);
                API.SetScenarioPedDensityMultiplierThisFrame(0.0f, 0.0f);
                API.SetVehicleDensityMultiplierThisFrame(0.0f);
                API.SetRandomVehicleDensityMultiplierThisFrame(0.0f);
                API.SetParkedVehicleDensityMultiplierThisFrame(0.0f);

                CheckCheckpoints();
            }


            await Task.FromResult(0);
        }

        /* -------------------------------------------------------------------------- */
        /*                               Vehicle Control                              */
        /* -------------------------------------------------------------------------- */
        private async Task<Vehicle> SpawnRunnerCar(string vehicle)
        {
            if (Util.IsPossibleCar(vehicle))
            {
                Util.RequestModel(vehicle);
                Vehicle car = await World.CreateVehicle(vehicle, this.spawn, this.heading);
                Util.SetCarLicensePlate(car, "RUNNER");

                return car;
            }
            else
            {
                return null;
            }
        }

        private async Task<Vehicle> SpawnBlockerCar(string vehicle)
        {
            if (Util.IsPossibleCar(vehicle))
            {
                Util.RequestModel(vehicle);
                Vehicle car = await World.CreateVehicle(vehicle, this.spawn, this.heading);
                Util.SetCarLicensePlate(car, "BLOCKER");

                return car;
            }
            else
            {
                return null;
            }
        }

        private async Task<Vehicle> SpawnCar()
        {
            Vehicle car;
            if (this.role == "Runner")
            {
                car = await SpawnRunnerCar(this.runnerCar);
            }
            else
            {
                car = await SpawnBlockerCar(this.blockerCar);
            }

            TeamColor carColor = TeamColors.list[this.teamColor];
            Util.SetCarColor(car, carColor.r, carColor.g, carColor.b);

            car.LockStatus = VehicleLockStatus.StickPlayerInside;
            car.IsExplosionProof = true;
            car.IsEngineRunning = true;

            car.PlaceOnGround();

            this.myCar = car;
            return car;
        }

        public void DestroyCar()
        {
            this.myCar.Delete();
        }

        public void SetCarTypes(string runnerCar, string blockerCar)
        {
            if (runnerCar == "")
            {
                this.runnerCar = "Voodoo";
            }
            else
            {
                this.runnerCar = runnerCar;
            }

            if (blockerCar == "")
            {
                this.blockerCar = "Insurgent2";
            }
            else
            {
                this.blockerCar = blockerCar;
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                                Spawn Control                               */
        /* -------------------------------------------------------------------------- */
        public void SetSpawn(Vector3 newSpawn, float newHeading)
        {
            this.spawn = newSpawn;
            this.heading = newHeading;
        }

        public async void RespawnPlayer()
        {
            DestroyCar();
            await SpawnCar();
        }

        private void PreparePlayer()
        {
            Ped player = Game.Player.Character;
            player.SetIntoVehicle(this.myCar, VehicleSeat.Driver);
            player.IsCollisionEnabled = true;
            player.IsVisible = true;

            Game.Player.CanControlCharacter = false;
        }

        /* -------------------------------------------------------------------------- */
        /*                                 Player Info                                */
        /* -------------------------------------------------------------------------- */
        public void SetRole(string newRole)
        {
            this.role = newRole;
        }

        public string GetRole()
        {
            return this.role;
        }

        public void SetTeamColor(string newColor)
        {
            this.teamColor = newColor;
        }

        public string GetTeamColor()
        {
            return this.teamColor;
        }

        /* -------------------------------------------------------------------------- */
        /*                                 Checkpoints                                */
        /* -------------------------------------------------------------------------- */
        public void SetCheckpoints(List<Vector3> checkpoints)
        {
            this.checkpoints = checkpoints;
            this.completedCheckpoints = new bool[checkpoints.Count];
            for (int i = 0; i < completedCheckpoints.Length; i++)
            {
                this.completedCheckpoints[i] = false;
            }
        }

        private void CheckCheckpoints()
        {
            if (role == "Runner")
            {
                Vector3 playerLoc = Game.Player.Character.Position;
                int checkpointNum = 0;
                foreach (Vector3 cp in this.checkpoints)
                {
                    if (!this.completedCheckpoints[checkpointNum])
                    {
                        if (API.Vdist2(playerLoc.X, playerLoc.Y, playerLoc.Z, cp.X, cp.Y, cp.Z) < (4 * 1.12))
                        {
                            this.completedCheckpoints[checkpointNum] = true;
                            CreateCheckpointAndBlip();

                            TriggerServerEvent("OffDef:AddTeamPoint", this.teamColor);
                        }
                        checkpointNum++;
                    }
                }
            }
        }

        private void CreateCheckpointAndBlip()
        {
            if (currentBlip != null)
            {
                currentBlip.Delete();
            }

            if (currentCheckpoint != -100)
            {
                API.DeleteCheckpoint(currentCheckpoint);
            }

            if (this.gameActive)
            {
                int currentCheckpointIndex = -1;
                for (int i = 0; i < this.completedCheckpoints.Length; i++)
                {
                    if (this.completedCheckpoints[i] == false)
                    {
                        currentCheckpointIndex = i;
                        break;
                    }
                }

                if (currentCheckpointIndex != -1)
                {
                    Vector3 currentCheckpointPos = this.checkpoints[currentCheckpointIndex];
                    if (currentCheckpointIndex == this.checkpoints.Count - 1)
                    {
                        currentCheckpoint = API.CreateCheckpoint(4, currentCheckpointPos.X, currentCheckpointPos.Y, currentCheckpointPos.Z, currentCheckpointPos.X, currentCheckpointPos.Y, currentCheckpointPos.Z, 4.0f, 255, 255, 0, 255, 0);
                    }
                    else
                    {
                        Vector3 nextCheckpointPos = this.checkpoints[currentCheckpointIndex + 1];
                        currentCheckpoint = API.CreateCheckpoint(0, currentCheckpointPos.X, currentCheckpointPos.Y, currentCheckpointPos.Z, nextCheckpointPos.X, nextCheckpointPos.Y, nextCheckpointPos.Z, 4.0f, 255, 255, 0, 255, 0);
                    }

                    currentBlip = World.CreateBlip(currentCheckpointPos);
                    currentBlip.Color = BlipColor.Yellow;
                    currentBlip.Sprite = BlipSprite.Standard;
                    currentBlip.ShowRoute = true;
                }
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                                General Game                                */
        /* -------------------------------------------------------------------------- */
        public async void StartGame()
        {
            Util.SendChatMsg("New Offense Defense Game is Starting!", 0, 255, 255);
            await SpawnCar();
            PreparePlayer();
            this.gameActive = true;
            CreateCheckpointAndBlip();

            Debug.WriteLine("Client Ready!");
            TriggerServerEvent("OffDef:ClientReady", Game.Player.Name);
        }

        private void DisableControls()
        {
            Game.DisableControlThisFrame(1, Control.VehicleExit);
            Game.DisableControlThisFrame(1, Control.VehicleAttack);
        }

        private void PostCountdown()
        {
            Game.Player.CanControlCharacter = true;
        }

        public void EndGame(string winningTeam)
        {
            this.winningTeam = winningTeam;
            this.gameOver = true;

            this.gameActive = false;
            DrawEndGame();

        }

        private void UpdateScoreboard(dynamic ranks)
        {
            Payload payload = new Payload();
            payload.scoreboadEnable = true;
            payload.scoreboardPayload = ranks;

            Util.SendNuiMessage(payload);
        }

        /* -------------------------------------------------------------------------- */
        /*                                     UI                                     */
        /* -------------------------------------------------------------------------- */
        private void DrawCountdown()
        {
            string outString = "";

            if (this.currentCount == 0)
            {
                outString = "GO!";
                Util.SendChatMsg(outString, 0, 255, 0);
            }
            else
            {
                outString = this.currentCount.ToString();
                Util.SendChatMsg(outString, 255, 255, 0);
            }

        }

        private void DrawEndGame()
        {
            if (this.gameOver)
            {
                string outString = $"Winner: {this.winningTeam}";
                TeamColor winningColor = TeamColors.list[this.winningTeam];

                Util.SendChatMsg(outString, winningColor.r, winningColor.g, winningColor.b);
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                               Event Handlers                               */
        /* -------------------------------------------------------------------------- */
        private void SendCountdownTimer(int count)
        {
            this.currentCount = count;
            DrawCountdown();

            if (count == 0)
            {
                PostCountdown();
            }
        }
    }
}