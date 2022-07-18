$(function () {
  window.addEventListener("message", function (event) {
    let data = event.data;

    // Team Configuration Menu
    if (data.configEnable) {
      this.document.getElementById("teamConfig").style.display = "block";
      let field = this.document.getElementById("statusField");
      if (data.configLock) {
        field.innerText = "LOCKED";
        field.style.color = "red";
      } else {
        field.innerText = "";
        field.style.color = "black";
      }

      // Fill in the team data
      if (data.configPayload != null) {
        fillInMenu(data.teams.blue);
        fillInMenu(data.teams.red);
        fillInMenu(data.teams.green);
        fillInMenu(data.teams.orange);
        fillInMenu(data.teams.yellow);
        fillInMenu(data.teams.pink);
        fillInMenu(data.teams.purple);
        fillInMenu(data.teams.white);
      }
    } else {
      this.document.getElementById("teamConfig").style.display = "none";
    }

    // Game scoreboard
    if (data.scoreboardEnable) {
      this.document.getElementById("scoreboard").style.display = "block";

      // Fill the table with the scores
      if (data.scoreboardPayload != null) {
        let table = this.document.getElementById("scoreboardTable");
        for (let i = 0; i < data.scores.length; i++) {
          let teamScore = data.scores[i];
          let newRow = table.insertRow(-1);

          let positionItem = this.document.createElement("td");
          let teamItem = this.document.createElement("td");
          let scoreItem = this.document.createElement("td");

          positionItem.innerHTML = teamScore.position;
          teamItem.innerHTML = teamScore.team;
          scoreItem.innerHTML = teamScore.score;

          newRow.appendChild(positionItem);
          newRow.appendChild(teamItem);
          newRow.appendChild(scoreItem);
        }
      }
    } else {
      this.document.getElementById("scoreboard").style.display = "none";
    }

    // Create Game Menu
    if (data.createGameEnable) {
      this.document.getElementById("gameMenu").style.display = "block";

      if (data.createGamePayload != null) {
      }
    } else {
      this.document.getElementById("gameMenu").style.display = "none";
    }
  });
});

function fillInMenu(team) {
  $(`#${team.color} #runner`).text(team.runner);

  let blockers = "";
  if (team.blockers.length > 0) {
    blockers = team.blockers[0];
    for (i = 1; i < team.blockers.length; i++) {
      blockers = `${blockers}, ${team.blockers[i]}`;
    }
  }
  $(`#${team.color} #blocker`).text(blockers);
}
