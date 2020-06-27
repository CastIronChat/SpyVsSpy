using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// TODO how are collectibles represented visually?
/// When a player picks one up, how do we move it into their inventory?
/// Hide the GameObject and store a reference in the player's inventory?
public class Collectible : MonoBehaviour
{
    public CollectibleType type;
}
