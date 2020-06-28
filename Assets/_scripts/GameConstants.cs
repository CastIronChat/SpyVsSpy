using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Not sure if this makes sense, but we can sync game constants so we can tweak values while playing
public class GameConstants : MonoBehaviour
{
    public bool playerRespawnsAreLimited = false;
    public int playerMaxDeaths = 3;
    public int playerMaxInventorySpace = 3;
    /// Number of extra inventory slots granted by having the briefcase
    public int briefcaseGrantsExtraInventorySpace = 1;

    public int roundMaxTimeSeconds = 60 * 3;
    public int startingTraps = 3;
    public bool roundTimeIsLimited = true;
    private TrapTypeRegistry _trapTypes;
    public TrapTypeRegistry trapTypes {
        get {
            if(_trapTypes == null) {
                var types = new List<TrapType>(GetComponentsInChildren<TrapType>());
                _trapTypes = new TrapTypeRegistry();
                foreach(var type in types) _trapTypes.addEntity(type);
            }
            return _trapTypes;
        }
    }
    private CollectibleTypeRegistry _collectibleTypes;
    // Grab array of child components
    public CollectibleTypeRegistry collectibleTypes {
        get {
            if(_collectibleTypes == null) {
                var types = new List<CollectibleType>(GetComponentsInChildren<CollectibleType>());
                _collectibleTypes = new CollectibleTypeRegistry();
                foreach(var type in types) _collectibleTypes.addEntity(type);
            }
            return _collectibleTypes;
        }
    }
}
