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
        private int currentCheckpoint;

        private string teamColor;
        private string role;

        private Vehicle myCar;

        // Countdown
        private const int timePerCountDown = 1000;
        private const int startingNumber = 5;
        private int currrentCountdownTime = 0;
        private bool countdownActive = false;
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

                DrawCountdown();

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

        private async void SpawnCar(string vehicle = "")
        {
            Vehicle car;
            if (this.role == "Runner")
            {
                if (vehicle == "")
                {
                    vehicle = "Voodoo";
                }
                car = await SpawnRunnerCar(vehicle);
            }
            else
            {
                if (vehicle == "")
                {
                    vehicle = "Insurgent2";
                }
                car = await SpawnBlockerCar(vehicle);
            }

            TeamColor carColor = TeamColors.list[this.teamColor];
            Util.SetCarColor(car, carColor.r, carColor.g, carColor.b);

            car.LockStatus = VehicleLockStatus.StickPlayerInside;
            car.IsExplosionProof = true;
            car.IsEngineRunning = true;

            car.PlaceOnGround();
            car.MarkAsNoLongerNeeded();

            this.myCar = car;
        }

        public void DestroyCar()
        {
            this.myCar.Delete();
        }

        /* -------------------------------------------------------------------------- */
        /*                                Spawn Control                               */
        /* -------------------------------------------------------------------------- */
        public void SetSpawn(Vector3 newSpawn, float newHeading)
        {
            this.spawn = newSpawn;
            this.heading = newHeading;
        }

        public void RespawnPlayer()
        {
            DestroyCar();
            SpawnCar();
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

        private void DrawCheckpoints()
        {
            if (role == "Runner" && this.gameActive)
            {
                int count = 0;
                foreach (Vector3 cp in this.checkpoints)
                {
                    if (!this.completedCheckpoints[count])
                    {
                        API.DrawMarker(1, cp.X, cp.Y, cp.Z, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 4.0f, 4.0f, 4.0f, 255, 255, 0, 255, false, true, 2, false, null, null, false);
                        if (count == checkpoints.Count - 1)
                        {
                            // Draw a sub-marker for the finish line symbol
                        }
                    }
                }
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
            currentBlip.Delete();
            API.DeleteCheckpoint(currentCheckpoint);
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
        public void StartGame()
        {
            SpawnCar();
            PreparePlayer();
            CreateCheckpointAndBlip();

            Debug.WriteLine("Client Ready!");
            TriggerServerEvent("OffDef:ClientReady", Game.Player.Name);
            this.gameActive = true;
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

        }

        /* -------------------------------------------------------------------------- */
        /*                                     UI                                     */
        /* -------------------------------------------------------------------------- */
        private void DrawCountdown()
        {
            if (this.countdownActive)
            {
                string outString = "";
                if (this.currentCount == 0)
                {
                    outString = "GO!";
                }
                else
                {
                    outString = this.currentCount.ToString();
                }

                API.SetTextFont(0);
                API.SetTextScale(0.6f, 0.6f);
                API.SetTextColour(255, 255, 255, 255);
                API.SetTextEntry("STRING");
                API.AddTextComponentString(outString);
                API.DrawText(0.5f, 0.5f);
            }
        }

        private void DrawEndGame()
        {
            if (this.gameOver)
            {
                string outString = $"Winner: {this.winningTeam}";
                TeamColor winningColor = TeamColors.list[this.winningTeam];

                API.SetTextFont(0);
                API.SetTextScale(0.6f, 0.6f);
                API.SetTextColour(winningColor.r, winningColor.g, winningColor.b, 255);
                API.SetTextEntry("STRING");
                API.AddTextComponentString(outString);
                API.DrawText(0.5f, 0.5f);
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                               Event Handlers                               */
        /* -------------------------------------------------------------------------- */
        private void SendCountdownTimer(int count)
        {
            if (count == -1)
            {
                this.countdownActive = false;
                PostCountdown();
            }
            else
            {
                this.currentCount = count;
                this.countdownActive = true;

            }
        }
    }
}