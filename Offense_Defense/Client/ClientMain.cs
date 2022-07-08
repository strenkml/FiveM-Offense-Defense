using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;

namespace OffenseDefense.Client
{
    public class ClientMain : BaseScript
    {
        public ClientMain()
        {
            Debug.WriteLine("Hi from OffenseDefense.Client!");
            World.CreateVehicle(VehicleHash.Akuma, new Vector3());
        }


        [Tick]
        public Task OnTick()
        {
            DrawRect(0.5f, 0.5f, 0.5f, 0.5f, 255, 255, 255, 150);
            
            return Task.FromResult(0);
        }
    }
}
