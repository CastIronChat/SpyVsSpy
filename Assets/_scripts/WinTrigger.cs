using System;
using UnityEngine;

/// <summary>
/// When the player walks into WinTrigger, they win the game, trigger the victory animation, and then the "restart" button.  Should only be possible for players who
/// have permission to open an exit door.
/// </summary>
public class WinTrigger : MonoBehaviour
{
    GameManager gm
    {
        get => GameManager.instance;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<Player>();
        if ( player != null && gm.winState.winner == null )
        {
            gm.BroadcastPlayerHitWinTrigger( player );
            // TODO use GameManager RPC to announce winner, start timer to show the "restart" button
        }
    }
}
