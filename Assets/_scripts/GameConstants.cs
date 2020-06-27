using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Not sure if this makes sense, but we can sync game constants so we can tweak values while playing
public class GameConstants : MonoBehaviour
{
    public bool playerRespawnsAreLimited = false;
    public int playerMaxDeaths = 3;
    public int playerMaxInventorySpace = 9;

    public int roundMaxTimeSeconds = 60 * 3;
    public int startingTraps = 3;
    public bool roundTimeIsLimited = true;
    public List<TrapType> _trapTypes;
    // Grab array of child components
    public List<TrapType> trapTypes {
        get {
            if(_trapTypes == null) {
                _trapTypes = new List<TrapType>(GetComponentsInChildren<TrapType>());
            }
            return _trapTypes;
        }
    }
    public List<CollectibleType> _collectibleTypes;
    // Grab array of child components
    public List<CollectibleType> collectibleTypes {
        get {
            if(_collectibleTypes == null) {
                _collectibleTypes = new List<CollectibleType>(GetComponentsInChildren<CollectibleType>());
            }
            return _collectibleTypes;
        }
    }
}
