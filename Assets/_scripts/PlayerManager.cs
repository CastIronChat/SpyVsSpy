using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // TODO currently there is duplication for tracking players between using transform children and using this registry.
    // In future, track via registry.
    public PlayerRegistry players = new PlayerRegistry();

    // private HashSet<Player> _inactivePlayers = new HashSet<Player>();
    // private HashSet<Player> _activePlayers = new HashSet<Player>();
    // private HashSet<Player> _allPlayers = new HashSet<Player>();
    public ReadOnlyCollection<Player> activePlayers {
        // get => _activePlayers;
        get => new ReadOnlyCollection<Player>(transform.GetComponentsInChildren<Player>());
    }
    public ReadOnlyCollection<Player> idlePlayers {
        // get => _inactivePlayers;
        get => new ReadOnlyCollection<Player>(GameManager.instance.idleplayerManager.transform.GetComponentsInChildren<Player>());
    }

    public Player getPlayerById(int playerId)
    {
        return players.First( p => p.playerId == playerId );
    }

    public Player localPlayer
    {
        get => players.First( p => p.photonView.isMine );
    }

    public int playerCount {
        get => players.Count;
    }

    public void add(Player player)
    {
        // TODO duplication between tracking players via the transform children and via the Registry.
        player.transform.parent = transform;
        players.add( player );
    }

    public void Start()
    {
        players.installAsSerializer();
    }
}
