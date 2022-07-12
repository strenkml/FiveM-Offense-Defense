$(function () {
  window.addEventListener("message", function (event) {
    let data = event.data;

    // Team Configuration Menu
    if (data.teamConfig != null) {
      if (data.teamConfig == true) {
        this.document.getElementById("teamConfig").style.display = "block";
        let field = this.document.getElementById("statusField");
        if (data.lockTeamConfig == true) {
          field.innerText = "LOCKED";
          field.style.color = red;
        } else {
          field.innerText = "";
          field.style.color = black;
        }

        // Fill in the team data
        if (data.teams != null) {
          // TODO: Add logic for filling in the table
        }

      } else {
        this.document.getElementById("teamConfig").style.display = "none";
      }
    }

    // Game scoreboard
    if (data.scoreboard != null) {
      if (data.scoreBoard == true) {
        this.document.getElementById("scoreboard").style.display = "block";
        // Fill the table with the scores
        if (data.scores != null) {
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
    }
  });
});
