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
      // TODO: Prepend all of the commands with od
      API.RegisterCommand("del", new Action(DeleteCars), false);
      API.RegisterCommand("showConfig", new Action(ShowMenu), false);
      API.RegisterCommand("hideConfig", new Action(HideMenu), false);
      API.RegisterCommand("joinTeam", new Action<String>(JoinTeam), false);
      API.RegisterCommand("leaveTeam", new Action(LeaveTeam), false);
      API.RegisterCommand("setRunner", new Action(JoinRunner), false);

      // NUI Callbacks
      // API.RegisterNuiCallbackType("exit");
      // EventHandlers["__cfx_nui:exit"] += new Action(() =>
      // {
      //     HideMenu();
      // });

      // Event Handlers
      EventHandlers.Add("OffDef:UpdateTeams", new Action<dynamic>(UpdateTeams));
      EventHandlers.Add("OffDef:StartGame", new Action<string, string>(StartGame));

    }

    // Command Methods
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
    private void JoinTeam(String teamColor)
    {
      string playerName = Game.Player.Name;

      API.TriggerServerEvent("OffDef:AddPlayer", teamColor, playerName);
    }

    private void LeaveTeam()
    {
      string playerName = Game.Player.Name;

      API.TriggerServerEvent("OffDef:RemovePlayer", playerName);
    }
    private void JoinRunner()
    {
      string playerName = Game.Player.Name;

      API.TriggerServerEvent("OffDef:SetRunner", playerName);
    }

    // Event Methods
    private void UpdateTeams(dynamic teams)
    {
      this.teams = teams;
      UpdateMenu();
    }

    private void StartGame(string color, string role)
    {
      Ped player = Game.Player.Character;
      if (role == "Runner")
      {
        MyGame.SpawnRunnerCar(player);
      }
      else
      {
        MyGame.SpawnBlockerCar(player);
      }
    }

    // NUI Methods
    private void UpdateMenu()
    {
      API.SendNuiMessage(JsonConvert.SerializeObject(new { teams = teams }));
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
