using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Responsibilities:
/// - Keep the "Not found" HUD up-to-date based on which collectibles are not currently
///   held by any players.
///
/// - Track who can/can't win based on holding all the collectibles or not.
///
/// - Answer queries about the above state, e.g., if a player tries to use an exit door, they're only allowed if they
///   have all the items.
/// </summary>
public class CollectibleManager : MonoBehaviour
{
    private Dictionary<Player, PlayerCollectibleState> playerStates = new Dictionary<Player, PlayerCollectibleState>();

    [FormerlySerializedAs("icons")]
    public IconRowHUD notFoundIcons;

    public PlayerCollectibleState getState(Player p)
    {
        PlayerCollectibleState state;
        if ( !playerStates.TryGetValue( p, out state ) )
        {
            playerStates[p] = state = new PlayerCollectibleState();
        }

        return state;
    }

    void Update()
    {
        // Discover all collectibles being held by a player
        var collectiblesHeldByAnyone = new HashSet<CollectibleType>();
        foreach ( var player in gm.playerManager.activePlayers )
        {
            var state = getState(player);
            int countHeldByThisPlayer = 0;
            foreach ( var pair in player.GetInventory().collectibles )
            {
                if ( pair.Value > 0 ) {
                    collectiblesHeldByAnyone.Add( pair.Key );
                    countHeldByThisPlayer++;
                }
            }
            state.hasAllCollectibles = countHeldByThisPlayer >= collectibleTypes.Count;
        }

        notFoundIcons.setCursorVisibility( false );
        // Iterate all possible collectibles, skipping the ones held, adding to UI
        var icon = 0;
        foreach ( var collectibleType in gm.gameConstants.collectibleTypes )
        {
            if ( !collectiblesHeldByAnyone.Contains( collectibleType ) )
            {
                notFoundIcons.setIconVisibility( icon, true );
                notFoundIcons.setIcon( icon, collectibleType.sprite );
                icon++;
            } else {
            }
        }
        for ( ; icon < notFoundIcons.getIconCount(); icon++ )
        {
            notFoundIcons.setIconVisibility( icon, false );
        }
    }

    private GameManager gm {get => GameManager.instance;}
    private PlayerManager pm {get => gm.playerManager;}
    private CollectibleTypeRegistry collectibleTypes
    {
        get => gm.gameConstants.collectibleTypes;
    }
}

public class PlayerCollectibleState
{
    public bool hasAllCollectibles = false;
}
