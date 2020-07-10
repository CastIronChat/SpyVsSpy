using System.Collections;
using System.Collections.Generic;
using CastIronChat.EntityRegistry;
using ExitGames.Client.Photon;
using UnityEngine;

/// Not sure if this makes sense, but we can sync game constants so we can tweak values while playing
public class GameConstants : MonoBehaviour
{
    [Description(@"
        In the future, we can sync all these values over the network.

        Might be useful for playtesting since we can reconfigure parameters from one client,
        and it'll affect the whole game.
    ")]
    public bool playerRespawnsAreLimited = false;
    public int playerMaxDeaths = 3;
    public int playerMaxInventorySpace = 3;
    /// Number of extra inventory slots granted by having the briefcase
    public int briefcaseGrantsExtraInventorySpace = 1;

    public int roundMaxTimeSeconds = 60 * 3;
    public int startingTraps = 3;
    public float poisonTime; //how long before a player takes damage from poison
    public bool roundTimeIsLimited = true;
    private TrapTypeRegistry _trapTypes;
    public TrapTypeRegistry trapTypes
    {
        get
        {
            if ( _trapTypes == null )
            {
                var types = new List<TrapType>( GetComponentsInChildren<TrapType>() );
                _trapTypes = new TrapTypeRegistry();
                _trapTypes.installAsSerializer();
                foreach ( var type in types ) _trapTypes.add( type );
            }
            return _trapTypes;
        }
    }
    private CollectibleTypeRegistry _collectibleTypes;
    // Grab array of child components
    public CollectibleTypeRegistry collectibleTypes
    {
        get
        {
            if ( _collectibleTypes == null )
            {
                var types = new List<CollectibleType>( GetComponentsInChildren<CollectibleType>() );
                _collectibleTypes = new CollectibleTypeRegistry();
                _collectibleTypes.installAsSerializer();
                foreach ( var type in types ) _collectibleTypes.add( type );
            }
            return _collectibleTypes;
        }
    }
    private static GlobalSingletonGetter<GameConstants> _instance = new GlobalSingletonGetter<GameConstants>();
    public static GameConstants instance {
        get => _instance.instance;
    }
}
