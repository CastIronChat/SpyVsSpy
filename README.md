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

---

# Setting up 2x Unity instances to run in parallel

Git clone another copy of the project.  For me, I have 2x peer copies of the project: ./SpyVsSpy and SpyVsSpy2

```
git clone https://github.com/CastIronChat/SpyVsSpy SpyVsSpy2
```

Then delete the `./SpyVsSpy2/Assets` and symlink it to the first project's `Assets` directory.
In PowerShell, looks something like this:

```
cd ./SpyVsSpy2
New-Item -Type SymbolicLink -Target C:\Users\cspotcode\Documents\Personal-dev\@CastIronChat\SpyVsSpy\Assets Assets
```

Might want to also symlink `Packages/manifest.json`

# Layers

0 -
1 -
2 - 
4 - player
5 -
6 -
7 - walls
