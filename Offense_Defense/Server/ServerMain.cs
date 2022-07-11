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
    public ServerMain()
    {
      Debug.WriteLine("Hi from OffenseDefense.Server!");

      foreach (string color in teamColors)
      {
        teams.Add(color, new Team(color));
      }

      // Event Handlers
      EventHandlers.Add("OffDef:AddPlayer", new Action<string, string>(AddPlayer));
      EventHandlers.Add("OffDef:RemovePlayer", new Action<string>(RemovePlayer));
      EventHandlers.Add("OffDef:SetRunner", new Action<string>(SetRunner));


    }

    private void AddPlayer(string color, string name)
    {
      string otherTeam = IsPlayerInOtherTeam(color, name);
      if (otherTeam != "")
      {
        Team oldTeam = this.teams[otherTeam];
        oldTeam.RemovePlayer(name);
      }
      Team team = this.teams[color];
      team.AddPlayer(name);
      API.TriggerClientEvent("OffDef:UpdateTeams", this.teams);
    }

    private void RemovePlayer(string name)
    {
      string color = GetPlayerTeam(name);
      if (color != "")
      {
        Team team = this.teams[color];
        team.RemovePlayer(name);
        API.TriggerClientEvent("OffDef:UpdateTeams", this.teams);
      }
    }

    private void SetRunner(string name)
    {
      string color = GetPlayerTeam(name);
      if (color != "")
      {
        Team team = this.teams[color];
        bool result = team.SetRole(name, "Runner");
        if (!result)
        {
          Debug.WriteLine($"ERROR: Failed to set player {name} to runner for {color} team");
        }
        API.TriggerClientEvent("OffDef:UpdateTeams", this.teams);
      }
    }

    private string IsPlayerInOtherTeam(string excludeColor, string name)
    {
      foreach (KeyValuePair<string, Team> entry in this.teams)
      {
        if (entry.Key != excludeColor)
        {
          if (entry.Value.IsPlayer(name))
          {
            return entry.Key;
          }
        }
      }
      return "";
    }

    private string GetPlayerTeam(string name)
    {
      foreach (KeyValuePair<string, Team> entry in this.teams)
      {
        if (entry.Value.IsPlayer(name))
        {
          return entry.Key;
        }
      }
      return "";
    }



    [Command("hello_server")]
    public void HelloServer()
    {
      Debug.WriteLine("Sure, hello.");
    }
  }
}
