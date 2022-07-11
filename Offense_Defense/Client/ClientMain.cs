using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace OffenseDefense.Client
{
  public class ClientMain : BaseScript
  {
    readonly string[] teamColors = { "blue", "red", "green", "orange", "yellow", "pink", "purple", "white" };
    dynamic teams;

    // Timers
    const int menuUpdateThreshold = 2000;
    int menuUpdateTime = 0;
    public ClientMain()
    {
      Debug.WriteLine("Hi from OffenseDefense.Client!");

      // Commands
      API.RegisterCommand("test", new Action(TestCommand), false);
      API.RegisterCommand("del", new Action(DeleteCars), false);
      API.RegisterCommand("show", new Action(ShowMenu), false);
      API.RegisterCommand("hide", new Action(HideMenu), false);
      API.RegisterCommand("joinTeam", new Action<String>(JoinTeam), false);

      // NUI Callbacks
      // API.RegisterNuiCallbackType("exit");
      // EventHandlers["__cfx_nui:exit"] += new Action(() =>
      // {
      //     HideMenu();
      // });

      // Event Handlers
      EventHandlers.Add("OffDef:UpdateTeams", new Action<dynamic>(UpdateTeams));

    }

    private async void TestCommand()
    {
      Ped player = Game.Player.Character;
      API.RequestModel((uint)VehicleHash.Banshee);
      Vehicle car = await World.CreateVehicle(VehicleHash.Apc, player.Position + (player.ForwardVector * 2));
      car.Mods.PrimaryColor = VehicleColor.Chrome;
    }

    private void DeleteCars()
    {
      Vehicle[] cars = World.GetAllVehicles();
      foreach (Vehicle car in cars)
      {
        car.Delete();
      }
    }

    private async void ShowMenu()
    {
      API.SendNuiMessage(JsonConvert.SerializeObject(new { enable = true }));
      API.SetNuiFocus(false, false);
    }
    private void HideMenu()
    {
      API.SendNuiMessage(JsonConvert.SerializeObject(new { enable = false }));
      API.SetNuiFocus(false, false);
    }

    private void UpdateMenu()
    {
      API.SendNuiMessage(JsonConvert.SerializeObject(new { teams = teams }));
    }

    private void JoinTeam(String teamColor)
    {
      string playerName = Game.Player.Name;

      API.TriggerServerEvent("OffDef:AddPlayer", teamColor, playerName);
    }

    private void UpdateTeams(dynamic teams)
    {
      this.teams = teams;
      UpdateMenu();
    }


    [Tick]
    public Task OnTick()
    {
      if (menuUpdateTime >= menuUpdateThreshold)
      {
        menuUpdateTime = 0;
        UpdateMenu();
      }
      else
      {
        menuUpdateTime++;
      }
      return Task.FromResult(0);
    }
  }
}
