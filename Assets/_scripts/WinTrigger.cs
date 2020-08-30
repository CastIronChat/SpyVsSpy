using System;
using UnityEngine;

/// <summary>
/// When the player walks into WinTrigger, they win the game, trigger the victory animation, and then the "restart" button.  Should only be possible for players who
/// have permission to open an exit door.
/// </summary>
public class WinTrigger : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        var player = other.gameObject.GetComponent<Player>();
        if ( player != null )
        {
            // TODO use GameManager RPC to announce winner, start timer to show the "restart" button
        }
    }
}
