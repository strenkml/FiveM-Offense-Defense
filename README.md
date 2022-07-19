# FiveM Offense Defense

Resource for FiveM that adds the Offense Defense gamemode that Rockstar removed from GTA Online.

## TODO

- [ ] Add logic to turn winners into spectators (Are completed blockers still supposed to be on the field?)
- [ ] Add a NUI menu for starting the game that is opened using the start game command
  - Pass in a list of Maps to choose from
- [ ] Create the maps
- [ ] Run through the logic flow and make sure that everything is connected correctly
- [ ] Remove the delete cars command
- [ ] Add OD prefix to all of the commands
- [ ] Complete Testing
- [x] Add logic for scoring and determining a winner
- [x] Gate all of the config editing commands to only work when there is not a lock on the config
- [x] Add logic for a runner hitting a checkpoint
- [x] Add a server event for when a player leaves the server
- [x] Create js code that parses the passed teams obj and fills in the config menu
- [x] Create events for when a player gets a point for their team
- [x] The startgame command should lock the config menu
- [x] Change all of the commands to use the correct argument format
