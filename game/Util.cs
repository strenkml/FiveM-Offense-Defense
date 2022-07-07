using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace OffDef
{
  class Util : BaseScript
  {
    public static void SetCarLicensePlate(Vehicle car, string text)
    {
      Function.Call(Hash.SET_VEHICLE_NUMBER_PLATE_TEXT, car.Handle, text);
    }

    public static void SetCarColor(Vehicle car, int r, int g, int b)
    {
      Function.Call(Hash.SET_VEHICLE_CUSTOM_PRIMARY_COLOR, car.Handle, r, g, b);
      Function.Call(Hash.SET_VEHICLE_CUSTOM_SECONDARY_COLOR, car.Handle, r, g, b);
    }
  }
}