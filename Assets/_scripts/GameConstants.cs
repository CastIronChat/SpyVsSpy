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
    public bool roundTimeIsLimited = true;
    public List<Sprite> trapSprites;
}
