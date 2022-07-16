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

        private string teamColor;
        private string role;

        private Vehicle myCar;

        // Countdown
        private const int timePerCountDown = 1000;
        private const int startingNumber = 5;
        private int currrentCountdownTime = 0;
        private bool countdownActive = false;
        private int currentCount = startingNumber;

        // Other Clients
        private bool allClientsReady = false;

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

        private void CheckCheckpoints()
        {
            if (role == "Runner")
            {

            }
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