using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// If there is anything unique about a trap, it can be stored as fields here.
/// Based on our discussion, traps are completely identical, even to the point that you
/// can set off your own trap, so traps don't need any sort of "owner" or identity.
/// So this class is currently empty and unused.
public class Trap : MonoBehaviour
{
    public TrapType type;
}
