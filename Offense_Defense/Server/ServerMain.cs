using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace OffenseDefense.Server
{
  public class ServerMain : BaseScript
  {
    readonly string[] teamColors = { "blue", "red", "green", "orange", "yellow", "pink", "purple", "white" };
    Dictionary<string, Team> teams = new Dictionary<string, Team>();

    Dictionary<int, string> players = new Dictionary<int, string>();
    public ServerMain()
    {
      Debug.WriteLine("Hi from OffenseDefense.Server!");

      foreach (string color in teamColors)
      {
        teams.Add(color, new Team(color));
      }

      // Event Handlers
      EventHandlers.Add("OffDef:StartConfig", new Action(StartConfig));
      EventHandlers.Add("OffDef:AddPlayer", new Action<string, string>(AddPlayer));
      EventHandlers.Add("OffDef:RemovePlayer", new Action<string>(RemovePlayer));
      EventHandlers.Add("OffDef:SetRunner", new Action<string>(SetRunner));
      EventHandlers.Add("OffDef:SetTeamSpawnLocation", new Action<Vector3, float>(SetTeamSpawn));


    }

    // Event Handler Methods
    private void StartConfig() {

    }
    private void AddPlayer(string color, string name)
    {
      string otherTeam = Util.IsPlayerInOtherTeam(color, name, teams);
      if (otherTeam != "")
      {
        Team oldTeam = this.teams[otherTeam];
        oldTeam.RemovePlayer(name);
      }
      Team team = this.teams[color];
      team.AddPlayer(name);
      TriggerClientEvent("OffDef:UpdateTeams", this.teams);
    }

    private void RemovePlayer(string name)
    {
      string color = Util.GetPlayerTeam(name, teams);
      if (color != "")
      {
        Team team = this.teams[color];
        team.RemovePlayer(name);
        TriggerClientEvent("OffDef:UpdateTeams", this.teams);
      }
    }

    private void SetRunner(string name)
    {
      string color = Util.GetPlayerTeam(name, teams);
      if (color != "")
      {
        Team team = this.teams[color];
        bool result = team.SetRole(name, "Runner");
        if (!result)
        {
          Debug.WriteLine($"ERROR: Failed to set player {name} to runner for {color} team");
        }
        TriggerClientEvent("OffDef:UpdateTeams", this.teams);
      }
    }

    private void SetTeamSpawn(Vector3 newSpawn, float heading) {
      
    }


    [Command("hello_server")]
    public void HelloServer()
    {
      Debug.WriteLine("Sure, hello.");
    }
  }
}
