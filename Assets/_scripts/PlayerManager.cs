using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerRegistry players = new PlayerRegistry();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

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

    public Player localPlayer
    {
        get => players.First( p => p.photonView.isMine );
    }

    public int playerCount {
        get => this.players.Count;
    }

}
