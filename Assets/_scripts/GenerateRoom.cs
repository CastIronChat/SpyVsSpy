using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GenerateRoom : MonoBehaviour
{
    public GameManager gameManager
    {
        get => GameManager.instance;
    }

    public SpriteRenderer wallSprite;
    public List<Sprite> roomDoorLayouts, floorPattern;
    public List<GameObject> hidingSpotPrefabs;
    public Transform newroomparent;
    public float roomradius = 2.75f;
    public int yCount = 0, xCount = 0;

    public List<GameObject> roomLayOutPrefabs;
    public List<int> activeRoomLayOutPrefabs;

    public Transform doors, walls, rooms, hazards, layouts, wholelayouts;
    void Start()
    {
        //int count = 0;
        //while (count < doors.childCount)
        //{
        //    doors.GetChild(count).name = "door" + count.ToString();
        //    walls.GetChild(count).name = "wall" + count.ToString();
        //    count++;
        //}
    }


    void Update()
    { __debugRandomize(); }


    /// <summary>
    /// Re-randomizes the map when debugging keybindings are presssed, or can be forced via method param
    /// </summary>
    public void __debugRandomize(bool generate = false)
    {

        if (gameManager.input.__debugRandomizeDoorsDown() || generate)
        {
            // disable hazards that might be already include, for playing multiple rounds
            //while(hazards.childCount > 0)
            //{
            //    Transform temp = hazards.GetChild(0);
            //    temp.parent = wholelayouts;
            //    temp.position = wholelayouts.position;
            //    temp.gameObject.active = false;
            //Destroy(hazards.GetChild(0).gameObject);
            //}
            //reset room layouts for playing multiple rounds
            //while (activeRoomLayOutPrefabs.Count > 0)
            //{
            //    roomLayOutPrefabs[activeRoomLayOutPrefabs[0]].active = false;
            //    activeRoomLayOutPrefabs.RemoveAt(0);

            //}
            RandomizeDoors();


        }


        if (gameManager.input.__debugRandomizeRoomSpotsDown() || generate)
        {
            RandomizeRoomSpots();
        }

    }

    public void RandomizeDoors()
    {
        int nonExitDoorsSet = 0;
        var _doors = doors.GetComponentsInChildren<Door>(true);
        for (int i = 0, l = _doors.Length; i < l; i++)
        {
            var door = _doors[i];
            if ( door.isExit )
            {
                continue;
            }

            nonExitDoorsSet++;
            if (Random.Range(0, 2.0f) > 1.5f)
            {
                gameManager.BroadCastDoorOrWall(i, false, true);

            }
            else
            { gameManager.BroadCastDoorOrWall(i, true, false); }
        }

        // Ensure there is only one exit
        // Exit doors use the new DoorOrWall pair, so we can query for them differently.
        // This logic will need to be changed if non-exit doors start using DoorOrWall pair, too, because we'll need
        // to filter the list based on which are exits and which are not.
        var exitDoors = doors.GetComponentsInChildren<DoorOrWall>(true);
        var chosenExitDoorIndex = Random.Range( 0, exitDoors.Length ) + nonExitDoorsSet;
        for ( int i = nonExitDoorsSet, l = exitDoors.Length + nonExitDoorsSet; i < l; i++ )
        {
            if ( i == chosenExitDoorIndex )
            {
                gameManager.BroadCastDoorOrWall( i, true, false );
            }
            else
            {
                gameManager.BroadCastDoorOrWall( i, false, true );
            }
        }
    }

    public void ResetHidingSpots()
    {
        foreach (Transform hazard in gameManager.map.hazards)
        {
            //    hazard.parent = wholelayouts;
            //    hazard.position = wholelayouts.position;
            //    hazard.gameObject.active = false;
            Destroy(hazard.gameObject);
        }
        foreach (Transform environment in gameManager.map.enviroment)
        {
            //    hazard.parent = wholelayouts;
            //    hazard.position = wholelayouts.position;
            //    hazard.gameObject.active = false;
            Destroy(environment.gameObject);
        }
        gameManager.hidingSpotManager.hidingSpots.Clear();
        //gameManager.hidingSpotManager.doorlistset = false;
        gameManager.hidingSpotManager.hidingspotlistset = false;
        int count = 0;
        while (count < roomLayOutPrefabs.Count)
        {
            foreach (Transform el in roomLayOutPrefabs[count].transform)
            {
                if (el.GetComponent<HidingSpot>() != null)
                {
                    el.GetComponent<HidingSpot>().SetTrap(null);
                    el.GetComponent<HidingSpot>().SetCollectible(0);
                }
            }
            //roomLayOutPrefabs[count].transform.parent = layouts;
            //roomLayOutPrefabs[count].active = false;


            count++;
        }
    }

    public void SetDoorOrWall(int whichspot, bool dooron, bool wallon)
    {
        if ( doors.childCount > whichspot )
        {
            var go = doors.GetChild( whichspot );
            var pair = go.GetComponent<DoorOrWall>();
            if ( pair != null )
            {
                pair.setTo( isDoor: dooron );
                Assert.AreEqual( dooron, !wallon, "wallon and dooron must be opposites" );
            }
            else
            {
                if ( doors.childCount > whichspot )
                {
                    doors.GetChild( whichspot ).gameObject.SetActive( dooron );
                }

                if ( walls.childCount > whichspot )
                {
                    walls.GetChild( whichspot ).gameObject.SetActive( wallon );
                }
            }
        }
    }


    //after randomization set the hiding spots and static pieces with a new object spawned from the list
    public void SetLayout(int whichlayout, Vector3 pos, float rot)
    {
        GameObject clone = Instantiate(roomLayOutPrefabs[whichlayout], pos, transform.rotation);
        clone.transform.Rotate(0, 0, rot);
        int count = clone.transform.childCount - 1;
        while (count >= 0)
        {
            if (clone.transform.GetChild(count).GetComponent<HidingSpot>() != null)
            {

                clone.transform.GetChild(count).parent = gameManager.map.hazards;
            }
            else { clone.transform.GetChild(count).parent = gameManager.map.enviroment; }
            count--;
        }
        //the clone is the parent holding the hiding spots and static enviroment pieces so it can be cleaned up after the room is set
        Destroy(clone);

    }

    public void RandomizeRoomSpots()
    {
        //check that it has a door connected to it

        gameManager.BroadcastResetHidingSpots();
        //while (activeRoomLayOutPrefabs.Count > 0)
        //{
        //    roomLayOutPrefabs[activeRoomLayOutPrefabs[0]].active = false;
        //    activeRoomLayOutPrefabs.RemoveAt(0);

        //}
        activeRoomLayOutPrefabs.Clear();
        int count = 0;
        while (activeRoomLayOutPrefabs.Count < rooms.childCount)
        {
            int rnd = Random.Range(0, roomLayOutPrefabs.Count);
            if (activeRoomLayOutPrefabs.Contains(rnd) == false)
            { activeRoomLayOutPrefabs.Add(rnd); }
        }

        foreach (Transform room in rooms)
        {
            bool validRoom = false;
            //if it is the main center large room, or there is a door connected to it, the room is valid
            if (room.GetComponent<Room>() != null && room.GetComponent<Room>().largeRoom == true )
            { validRoom = true; }
            else
            {
                foreach (Transform doorobj in doors)
                {
                    if (doorobj.gameObject.active == true && Vector3.Distance(doorobj.position, room.position) < 4  )
                    { validRoom = true; }
                }
            }


            if (validRoom == true)
            {
                GameObject newlayout = roomLayOutPrefabs[count];
                //GameObject clone = Instantiate(roomLayOutPrefabs[count], room.position, newlayout.transform.rotation);
                //newlayout.transform.position = room.position;
                float rndrotate = Random.Range(0, 2.0f);
                float roomrotation = 0;
                if (rndrotate < 5.0f)
                {
                    roomrotation = 270.0f;
                    newlayout.transform.Rotate(0, 0, 270);
                }
                else if (rndrotate < 1.0f) { newlayout.transform.Rotate(0, 0, 180); roomrotation = 180.0f; }
                else if (rndrotate < 1.5f) { newlayout.transform.Rotate(0, 0, 90); roomrotation = 90.0f; }
                else { }
                //activeRoomLayOutPrefabs.Add(newlayout);
                //templayouts.Remove(newlayout);

                gameManager.BroadcastRoomLayout(activeRoomLayOutPrefabs[count], room.position, roomrotation);
            }

            count++;
        }

        //RandomizeCollectibleSpots();

        gameManager.BroadcastSetHidingSpotAndDoorLists();
        //gameManager.BroadcastStartRound();
    }

    public void RandomizeCollectibleSpots()
    {
        int collectiblesleft = 4;
        int count = 0;
        //List<HidingSpot> tempHidingspots = new List<HidingSpot>();
        if (hazards.childCount <= collectiblesleft) { return; }
        //foreach (Transform el in hazards)
        //{
        //    foreach (Transform el2 in el)
        //    {
        //        if (el2.GetComponent<HidingSpot>() != null)
        //        {
        //            tempHidingspots.Add(el2.GetComponent<HidingSpot>());
        //        }
        //    }
        //}
        //if (tempHidingspots.Count <= collectiblesleft) { return; }
        while (collectiblesleft > 0 && count < 40)
        {
            count++;
            int rnd = Random.Range(0, hazards.childCount);
            var hazard = hazards.GetChild( rnd );
            var hidingSpot = hazard.GetComponent<HidingSpot>();
            if (hidingSpot.GetCollectible() == 0)
            {
                hidingSpot.SetCollectible(collectiblesleft);

                gameManager.BroadcastHidingSpotCollectible(hidingSpot.GetPlaceInList(), collectiblesleft);
                Debug.Log(hazard.transform.position);
                collectiblesleft--;
            }
            else { Debug.Log(hidingSpot.GetPlaceInList()); }
        }
        RandomizeWeapons();
    }

    public void RandomizeWeapons()
    {
        int weaponsLeft = 4;
        int count = 0;
        //List<HidingSpot> tempHidingspots = new List<HidingSpot>();
        if (hazards.childCount <= weaponsLeft) { return; }

        while (weaponsLeft > 0 && count < 40)
        {
            count++;
            int rnd = Random.Range(0, hazards.childCount);
            var hazard = hazards.GetChild( rnd );
            var hidingSpot = hazard.GetComponent<HidingSpot>();
            if (hidingSpot.GetCollectible() == 0)
            {
                hidingSpot.SetCollectible(5);

                gameManager.BroadcastHidingSpotCollectible(hidingSpot.GetPlaceInList(), 5);
                Debug.Log(hazard.transform.position);
                weaponsLeft--;
            }
            else { Debug.Log(hidingSpot.GetPlaceInList()); }
        }

        gameManager.BroadcastStartRound();
    }

}
