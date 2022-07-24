using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace OffenseDefense.Client
{
    class Util : BaseScript
    {
        public static void SetCarLicensePlate(Vehicle car, string text)
        {
            Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT, car.Handle, text);
        }

        public static void SetCarColor(Vehicle car, int r, int g, int b)
        {
            Function.Call(Hash.SET_VEHICLE_CUSTOM_PRIMARY_COLOUR, car.Handle, r, g, b);
            Function.Call(Hash.SET_VEHICLE_CUSTOM_SECONDARY_COLOUR, car.Handle, r, g, b);
        }

        public static bool WasButtonPressed(int button)
        {
            return API.IsControlJustReleased(1, button);
        }

        public static bool IsButtonPressed(int button)
        {
            return API.IsControlPressed(1, button);
        }

        public static bool SendNuiMessage(Payload payload)
        {
            Debug.WriteLine("Sending NUI Message");
            return API.SendNuiMessage(JsonConvert.SerializeObject(payload));
        }

        public static void GetPlayerDetails(string jsonDetails, out string role, out string color, out Vector3 spawn, out float spawnHeading, out List<Vector3> checkpoints, out string runnerCar, out string blockerCar)
        {
            GameDetails details = JsonConvert.DeserializeObject<GameDetails>(jsonDetails);
            role = details.role;
            color = details.color;
            spawn = details.spawn;
            spawnHeading = details.heading;
            checkpoints = details.checkpoints;
            runnerCar = details.runnerCar;
            blockerCar = details.blockerCar;
        }

        public static void RequestModel(string name)
        {
            uint hash = (uint)API.GetHashKey(name);
            API.RequestModel(hash);
        }

        public static void RequestModel(uint hash)
        {
            API.RequestModel(hash);
        }

        public static bool IsPossibleCar(string name)
        {
            uint hash = (uint)API.GetHashKey(name);

            return API.IsModelAVehicle(hash) && (API.IsThisModelABike(hash) || API.IsThisModelACar(hash) || API.IsThisModelAnAmphibiousCar(hash) || API.IsThisModelAnAmphibiousQuadbike((int)hash) || API.IsThisModelAQuadbike(hash));

        }

        public static bool IsPossibleCar(uint hash)
        {
            return API.IsModelAVehicle(hash) && (API.IsThisModelABike(hash) || API.IsThisModelACar(hash) || API.IsThisModelAnAmphibiousCar(hash) || API.IsThisModelAnAmphibiousQuadbike((int)hash) || API.IsThisModelAQuadbike(hash));

        }

        public static void SendChatMsg(string msg, int r = 255, int g = 255, int b = 255)
        {
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { r, g, b },
                multiline = false,
                args = new[] { msg }
            });
        }
    }
}