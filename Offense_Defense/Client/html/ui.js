$(function() {
  // const color = {
  //   runner = null,
  //   blockers = [],
  // }
  
  // var data = {
  //   blue = color,
  //   red = color,
  //   green = color,
  //   orange = color,
  //   yellow = color,
  //   pink = color,
  //   purple = color,
  //   white = color,
  // }



  window.addEventListener("message", function(event) {
      console.log(event.data.players);
      if(event.data.enable == true) {
        document.body.style.display = "block";
      } else {
        document.body.style.display = "none";
      }

      // if (event.data.players.length > 0) {
      //   let playersString = event.data.players;
      //   for (i = 1; i < event.data.players.length; i++) {
      //     playersString = `${playersString}, ${event.data.players[i]}`
      //   }
      //   this.document.getElementById("waitingList").innerText = playersString;
      // }
      
  });

  // $("#exit").click(function () {
  //   console.log("Clicked!")
  //   fetch(`https://${GetParentResourceName()}/exit`, {
  //     method: 'POST',
  //     headers: {
  //         'Content-Type': 'application/json; charset=UTF-8',
  //     },
  //     body: JSON.stringify({})
  // }).then(resp => resp.json()).then(resp => console.log(resp));
  // });
});