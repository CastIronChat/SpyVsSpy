# CrashYourFriends

Can hold items in briefcase
Can hold items in arms
When not holding items, have knife out

Can stash items in hidingSpot
- only one item per hidingSpot
-

HidingSpot
- Has an attached trap

GameConstants
- playerRespawnsAreCapped
- playerMaxDeaths
- playerStartingTrapsCount
- playerSpeed() arg used to determine if player is encumbered or not

### MISC

[FormerlySerializedAs("yourVariablesPreviousName")]

## Inventory networking

Player's inventory is observable via Photon's standard syncing mechanism.
Traps are set via ordered messages: set trap UUID on target UUID

Items are moved via ordered messages:
- move item from player to hidingspot
- move item from hiding spot to player
