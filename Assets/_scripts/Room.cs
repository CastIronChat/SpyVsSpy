using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int collectiblesInRoom = 0;
    public bool localPlayerHasVisited = false,largeRoom,validRoom;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVisited(bool visited)
    { localPlayerHasVisited = visited; }

    public bool GetVisited( )
    { return localPlayerHasVisited; }

    public void ChangeCollectible(int increment)
    {
        collectiblesInRoom += increment;
        //if (collectiblesInRoom < 0) { collectiblesInRoom = 0; }
    }

    public int GetCollectiblesCount()
    { return collectiblesInRoom; }

}
