using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Responsibilities:
/// Keep the "Not found" HUD up-to-date based on which collectibles are not currently
/// held by any players.
public class McGuffinHUD : MonoBehaviour
{
    private GameManager _gm;
    GameManager gm {
        get {
            if(_gm == null) _gm = GameManager.getGlobalSingletonGameManager();
            return _gm;
        }
    }
    public IconRowHUD icons;
    void Update() {
        // Discover all collectibles being held by a player
        var collectiblesHeld = new HashSet<CollectibleType>();
        foreach(var player in gm.playerManager.activePlayers().ToEnumerable()) {
            foreach(var collectible in player.GetInventory().collectibles) {
                if(collectible.Value > 0)
                    collectiblesHeld.Add(collectible.Key);
            }
        }

        icons.setCursorVisibility(false);
        // Iterate all possible collectibles, skipping the ones held, adding to UI
        var icon = 0;
        foreach(var collectibleType in gm.gameConstants.collectibleTypes) {
            if(!collectiblesHeld.Contains(collectibleType)) {
                icons.setIconVisibility(icon, true);
                icons.setIcon(icon, collectibleType.sprite);
            }
        }
        for(; icon < icons.getIconCount(); icon++) {
            icons.setIconVisibility(icon, false);
        }
    }
}
