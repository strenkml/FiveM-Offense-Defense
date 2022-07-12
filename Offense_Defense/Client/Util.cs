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

        public static void RequestModel(dynamic hash)
        {
            API.RequestModel((uint)hash);
        }

        public static bool WasButtonPressed(int button)
        {
            return API.IsControlJustReleased(1, button);
        }

        public static bool IsButtonPressed(int button)
        {
            return API.IsControlPressed(1, button);
        }

        public static bool SendNuiMessage(object obj)
        {
            return API.SendNuiMessage(JsonConvert.SerializeObject(obj));
        }

        public static void GetPlayerDetails(Dictionary<int, dynamic> details, out string role, out string color)
        {
            int playerID = API.PlayerId();
            role = "";
            color = "";

            foreach (KeyValuePair<int, dynamic> detail in details)
            {
                if (detail.Key == playerID)
                {
                    role = detail.Value.role;
                    color = detail.Value.color;
                }
            }
        }
    }
}