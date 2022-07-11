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
        readonly string[] teamColors = {"blue", "red", "green", "orange", "yellow", "pink", "purple", "white"};
        List<Team> teams;
        public ClientMain()
        {
            Debug.WriteLine("Hi from OffenseDefense.Client!");
            
            // Init the teams info
            teams = new List<Team>();
            foreach(string color in teamColors) {
                teams.Add(new Team(color));
            }
            
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

        }

        private async void TestCommand() {
            Ped player = Game.Player.Character;
            API.RequestModel((uint) VehicleHash.Banshee);
            Vehicle car = await World.CreateVehicle(VehicleHash.Apc, player.Position + (player.ForwardVector * 2));
            car.Mods.PrimaryColor = VehicleColor.Chrome;
        }

        private void DeleteCars() {
            Vehicle[] cars = World.GetAllVehicles();
            foreach(Vehicle car in cars) {
                car.Delete();
            }
        }

        private async void ShowMenu() {
            // dynamic playerIds = API.GetActivePlayers();
            // List<string> playerNames = new List<string>();
            // for (int i = 0; i < playerIds.Count; i++) {
            //     playerNames.Add(API.GetPlayerName(playerIds[i]));
            // }
            API.SendNuiMessage(JsonConvert.SerializeObject(new {enable = true}));
            API.SetNuiFocus(false, false);
        }
        private void HideMenu() {
            API.SendNuiMessage(JsonConvert.SerializeObject(new {enable = false}));
            API.SetNuiFocus(false, false);
        }

        private void UpdateMenu() {
            API.SendNuiMessage(JsonConvert.SerializeObject(new {teams = teams}));
        }

        private void JoinTeam(String teamColor) {
            // Current issue is finding a player on a team when they are switching teams
            // The team data structure needs to be rethought
            string playerName = Game.Player.Name;
            Team selectedTeam = teams.Find(e => e.GetColor() == teamColor);
            List<string> oldBlockers = selectedTeam.GetBlockers();
            List<string> newBlockers = new List<string>();
            foreach(string blocker in oldBlockers) {
                if (blocker == playerName) {
                    // Already part of the team
                    // Do nothing or send error to player
                    return;
                } else {
                    newBlockers.Add(blocker);
                }
            }
        }


        [Tick]
        public Task OnTick()
        {
            // DrawRect(0.5f, 0.5f, 0.5f, 0.5f, 255, 255, 255, 150);
            
            return Task.FromResult(0);
        }
    }
}
