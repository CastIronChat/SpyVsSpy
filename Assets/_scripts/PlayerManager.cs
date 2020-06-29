using System.Collections;
using System.Collections.Generic;
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

    public IEnumerator<Player> activePlayers() {
        foreach(var t in transform.getChildTransformEnumerator().ToEnumerable()) {
            var player = t.GetComponent<Player>();
            Debug.Assert(player != null);
            yield return player;
        }
    }

    public int activePlayerCount {
        get => transform.childCount;
    }

    public int playerCount {
        get => this.players.Count;
    }
}
