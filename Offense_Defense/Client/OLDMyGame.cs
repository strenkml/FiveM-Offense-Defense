using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace OffenseDefense.Client
{
    class OLDMyGame : BaseScript
    {
        private Vector3 spawn = new Vector3();
        private float spawnHeading;

        public string teamColor = "";

        public OLDMyGame()
        {
            Tick += onTick;
        }

        private async Task onTick()
        {
            await Task.FromResult(0);
        }

        private void DisableControls()
        {
            Game.DisableControlThisFrame(1, Control.VehicleExit);
            Game.DisableControlThisFrame(1, Control.VehicleAttack);
        }

        public void SetSpawn(Vector3 newSpawn, float heading)
        {
            spawn = newSpawn;
            spawnHeading = heading;
        }

        public async void NewOffDefGame()
        {
            await PreGame();

            // Vehicle runnerCar = await SpawnRunnerCar();
            //PreparePlayer(runnerCar);
            //await StartCountdown(runnerCar);
        }

        private async Task PreGame()
        {
            // Ped player = Game.Player.Character;


        }

        public async Task<Vehicle> SpawnRunnerCar(Ped driver, string color)
        {
            Vehicle car = await World.CreateVehicle(VehicleHash.Voodoo, driver.Position, driver.Heading);
            Util.SetCarLicensePlate(car, "RUNNER");
            Color carColor = Colors.list[color];
            Util.SetCarColor(car, carColor.r, carColor.g, carColor.b);

            car.Heading = spawnHeading;
            car.LockStatus = VehicleLockStatus.StickPlayerInside;
            car.IsExplosionProof = true;
            car.IsEngineRunning = true;

            car.PlaceOnGround();

            driver.SetIntoVehicle(car, VehicleSeat.Driver);



            return car;
        }

        public async Task<Vehicle> SpawnBlockerCar(Ped driver, string color)
        {
            Vehicle car = await World.CreateVehicle(VehicleHash.Insurgent2, driver.Position, driver.Heading);
            Util.SetCarLicensePlate(car, "BLOCKER");
            Color carColor = Colors.list[color];
            Util.SetCarColor(car, carColor.r, carColor.g, carColor.b);

            car.Heading = spawnHeading;
            car.LockStatus = VehicleLockStatus.StickPlayerInside;
            car.IsExplosionProof = true;
            car.IsEngineRunning = true;

            car.PlaceOnGround();

            driver.SetIntoVehicle(car, VehicleSeat.Driver);

            return car;
        }

        private void IncreaseTeamScore(int teamNum)
        {
            if (teamNum > 0 && teamNum < teamScores.Length)
            {
                teamScores[teamNum]++;
            }
            else
            {
                Console.WriteLine("Error: Invalid team number");
            }
        }

        private int GetTeamScore(int teamNum)
        {
            return teamScores[teamNum];
        }

        private void UpdateScoreboard()
        {

        }

        public void RespawnPlayer()
        {

        }
    }
}