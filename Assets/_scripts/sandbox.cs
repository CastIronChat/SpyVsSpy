using System.Collections;
using System.Collections.Generic;
using UnityEngine;


struct roomobj
{
    bool set;
    bool north, south, east, west;
}
public class sandbox : MonoBehaviour
{
    public Transform parentObj;
    public GameObject spawnthis;

    public Transform roomwithwalls, roomsnowalls;
    // Start is called before the first frame update
    void Start()
    {
        SpawnChildObjects();
        // layoutingrid();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnChildObjects()
    {
        foreach ( Transform go in parentObj )
        {
            Destroy( go.GetChild( 0 ).gameObject );
            GameObject clone = Instantiate( spawnthis, go.position, go.rotation ) as GameObject;
            clone.transform.parent = go;
            clone.transform.position = go.position;
        }
    }
    public void layoutingrid()
    {
        int count = 0;
        int count1 = 0;
        int count2 = 0;
        foreach ( Transform go in parentObj )
        {
            go.transform.localPosition = new Vector3( count1, 0, count2 );
            count++;
            count1++;

            if ( count1 > 9 )
            { count1 = 0; count2++; }
        }
    }
}
