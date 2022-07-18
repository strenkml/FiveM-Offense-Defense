using System;
using System.Threading.Tasks;
using CitizenFX.Core;
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

        /* -------------------------------------------------------------------------- */
        /*                                 Constructor                                */
        /* -------------------------------------------------------------------------- */
        public OffDefGame()
        {
            Tick += onTick;

            // Events
            EventHandlers.Add("OffDef:CountdownTimer", new Action<int>(SendCountdownTimer));
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
                DrawCountdown();

                DrawCheckpoints();
                DrawBlips();

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

            Color carColor = Colors.list[this.teamColor];
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

        private void DrawCheckpoints(List<Vector3> checkpoints)
        {
            if (role == "Runner")
            {
                int count = 0;
                foreach (Vector3 cp in checkpoints)
                {
                    if (!this.completedCheckpoints[count])
                    {
                        API.DrawMarkers(1, cp.x, cp.y, cp.z, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 4.0f, 4.0f, 4.0f, 255, 255, 0, 255, false, true, 2, null, null, false);
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
                Vector3 playerLoc = Game.Player.Charactor.Position;
                int checkpointNum = 0;
                foreach (Vector3 cp in this.checkpoints)
                {
                    if (!this.completedCheckpoints[i])
                    {
                        if (API.Vdist2(playerLoc.x, playerLoc.y, playerLoc.z, cp.x, cp.y, cp.z) < 4 * 1.12)
                        {
                            this.completedCheckpoints[i] = true;
                            TriggerServerEvent("OffDef:AddTeamPoint", this.teamColor);
                        }
                        checkpointNum++;
                    }
                }
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                                    Blips                                   */
        /* -------------------------------------------------------------------------- */
        private void DrawBlips()
        {
            int currentCheckpointIndex = -1;
            for (int i = 0; i < this.completedCheckpoints; i++)
            {
                if (this.completedCheckpoints[i] == false)
                {
                    currentCheckpointIndex = i;
                    break;
                }
            }

            if (currentCheckpointIndex != -1)
            {
                Vector3 currentCheckpoint = this.checkpoints.At(currentCheckpointIndex);
                Blip blip = API.AddBlipForCoord(currentCheckpoint.x, currentCheckpoint.y, currentCheckpoint.z);
                API.SetBlipColour(blip, 28);
                API.SetBlipDisplay(blip, 5);
                API.SetBlipRoute(blip, true);
                API.SetBlipSprite(blip, 1);
            }
        }

        /* -------------------------------------------------------------------------- */
        /*                                General Game                                */
        /* -------------------------------------------------------------------------- */
        public void StartGame()
        {
            SpawnCar();
            PreparePlayer();

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