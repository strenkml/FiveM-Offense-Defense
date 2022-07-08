using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace OffenseDefense.Client 
{
    class MyGame : BaseScript 
    {
        private static Vector3 spawn = new Vector3();
        private static float spawnHeading;
        private static Vector3 spectatePos = new Vector3();

        // Scoring
        const int numTeams = 8;
        static int[] teamScores = new int[numTeams];
        int numCheckpoints;


        public MyGame()
        {
            Tick += onTick;
        }

        private async Task onTick()
        {
            UpdateScoreboard();

            await Task.FromResult(0);
        }

        private void DisableControls()
        {
            Game.DisableControlThisFrame(1, Control.VehicleExit);
            Game.DisableControlThisFrame(1, Control.VehicleAttack);
        }

        public static void SetSpawn(Vector3 newSpawn, float heading, Vector3 pSpectatePos)
        {
            spawn = newSpawn;
            spawnHeading = heading;
            spectatePos = pSpectatePos;
        }

        public static async void NewOffDefGame()
        {
            await PreGame();

            Vehicle runnerCar = await SpawnRunnerCar();
            //PreparePlayer(runnerCar);
            //await StartCountdown(runnerCar);
        }

        private static async Task PreGame()
        {
            // Ped player = Game.Player.Character;
            

        }

        private static async Task<Vehicle> SpawnRunnerCar()
        {
            Vehicle car = await World.CreateVehicle(VehicleHash.Voodoo, spawn);
            Util.SetCarLicensePlate(car, "RUNNER");
            Color carColor = Colors.blue;
            Util.SetCarColor(car, carColor.r, carColor.g, carColor.b);

            car.Heading = spawnHeading;
            car.LockStatus = VehicleLockStatus.StickPlayerInside;
            car.IsExplosionProof = true;
            car.IsEngineRunning = true;

            car.PlaceOnGround();

            

            return car;
        }
        
        private static async Task<Vehicle> SpawnBlockerCar()
        {
            Vehicle car = await World.CreateVehicle(VehicleHash.Insurgent2, spawn);
            Util.SetCarLicensePlate(car, "BLOCKER");
            Color carColor = Colors.blue;
            Util.SetCarColor(car, carColor.r, carColor.g, carColor.b);

            car.Heading = spawnHeading;
            car.LockStatus = VehicleLockStatus.StickPlayerInside;
            car.IsExplosionProof = true;
            car.IsEngineRunning = true;

            car.PlaceOnGround();

            return car;
        }

        private static void IncreaseTeamScore(int teamNum)
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

        private static int GetTeamScore(int teamNum)
        {
            return teamScores[teamNum];
        }

        private static void UpdateScoreboard()
        {

        }
    }
}