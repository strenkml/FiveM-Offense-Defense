$(function () {
  var errors = [];

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
        fillInMenu(data.configPayload.blue);
        fillInMenu(data.configPayload.red);
        fillInMenu(data.configPayload.green);
        fillInMenu(data.configPayload.orange);
        fillInMenu(data.configPayload.yellow);
        fillInMenu(data.configPayload.pink);
        fillInMenu(data.configPayload.purple);
        fillInMenu(data.configPayload.white);
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
          console.log(
            `Pos: ${teamScore.positionItem} Team: ${teamScore.team} Score: ${teamScore.scoreItem}`
          );

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
    } else {
      this.document.getElementById("gameMenu").style.display = "none";
    }
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

  $("#start").click(function () {
    let mapValue = $("#mapSelect").val();
    let runnerValue = $("#runnerSelect").val();
    let blockerValue = $("#blockerSelect").val();

    if (mapValue.length == 0) {
      fetch(`https://${GetParentResourceName()}/error`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json; charset=UTF-8",
        },
        body: JSON.stringify({
          error: "A map needs to be given",
        }),
      })
        .then((resp) => resp.json())
        .then((resp) => console.log(resp));
      return;
    }

    fetch(`https://${GetParentResourceName()}/startGame`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json; charset=UTF-8",
      },
      body: JSON.stringify({
        map: mapValue,
        runner: runnerValue,
        blocker: blockerValue,
      }),
    })
      .then((resp) => resp.json())
      .then((resp) => console.log(resp));
    return;
  });
});
