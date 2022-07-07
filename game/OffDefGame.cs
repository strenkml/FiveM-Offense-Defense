using System;
using System.Collections.Generic;
using CitizenFX.Core;
using static CitizenFX.Core.Native.API;

namespace OffDef
{
  public class OffDefGame : BaseScript
  {
    private static Vector3 spawn = new Vector3();
    private static float spawnHeading;
    private static Vector3 spectatePos = new Vector3();
    public OffDefGame()
    {
      Tick += onTick;
    }

    private async Task onTick()
    {

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
      PreparePlayer(runnerCar);
      await StartCountdown(runnerCar);
    }

    private static async Task PreGame()
    {
      Ped playerPed = Game.PlayerPed;

    }

    private static async Task<Vehicle> spawnRunnerCar()
    {
      Vehicle car = await World.CreateVehicle(VehicleHash.voodoo);
      Util.SetCarLicensePlate(car, "RUNNER");
      Color carColor = Colors.blue;
      Util.SetCarColor(car, carColor.r, carColor.g, carColor.b);

      car.Heading = spawnHeading;
      car.LockStatus = VehicleLockStatus.StickPlayerInside;
      car.IsExplosionProof = true;
      car.IsEngineRunning = true;

      car.PlaceOnGround();
    }
    private static async Task<Vehicle> spawnBlockerCar()
    {
      Vehicle car = await World.CreateVehicle(VehicleHash.insurgent);
      Util.SetCarLicensePlate(car, "BLOCKER");
      Color carColor = Colors.blue;
      Util.SetCarColor(car, carColor.r, carColor.g, carColor.b);

      car.Heading = spawnHeading;
      car.LockStatus = VehicleLockStatus.StickPlayerInside;
      car.IsExplosionProof = true;
      car.IsEngineRunning = true;

      car.PlaceOnGround();
    }


  }
}