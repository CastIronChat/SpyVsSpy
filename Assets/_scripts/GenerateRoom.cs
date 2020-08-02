using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRoom : MonoBehaviour
{

    public GameManager gameManager
    {
        get => GameManager.instance;
    }

  public SpriteRenderer wallSprite;
    public List<Sprite> roomDoorLayouts,floorPattern;
    public List<GameObject> hidingSpotPrefabs;
    public GameObject roomPrefab,westCollider,eastCollider,northCollider,southCollider; //for when theyre isnt a door on that side
    public Transform newroomparent;
    public float roomradius = 2.75f;
    public int yCount = 0,xCount = 0;

    public List<GameObject> roomLayOutPrefabs;
    public List<int>  activeRoomLayOutPrefabs;

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
    {

        if (Input.GetKeyDown(KeyCode.I))
        {
            //foreach (Transform hazard in hazards)
            while(hazards.childCount > 0)
            {
                Transform temp = hazards.GetChild(0);
                temp.parent = wholelayouts;
                temp.position = wholelayouts.position;
                temp.gameObject.active = false;
                //Destroy(hazard.gameObject);
            }

            while (activeRoomLayOutPrefabs.Count > 0)
            {
                roomLayOutPrefabs[activeRoomLayOutPrefabs[0]].active = false;
                activeRoomLayOutPrefabs.RemoveAt(0);

            }
            RandomizeDoors();
          

        }
        if (Input.GetKeyDown(KeyCode.U))
        {

            foreach (Transform hazard in hazards)
            {
                hazard.parent = wholelayouts;
                hazard.position = wholelayouts.position;
                hazard.gameObject.active = false;
                //Destroy(hazard.gameObject);
            }
            RandomizeRoomSpots();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
          int count2 = 0;
          while(count2 < 20){
                yCount++;
                if(yCount > 4){yCount = 0; xCount++;}
                transform.position = new Vector3((6.75f * xCount), (yCount * 6.75f));
                Transform newroom = MakeNewRoom();
                newroom.parent = newroomparent;
                newroom.GetChild(2).GetComponent<SpriteRenderer>().sprite = floorPattern[ (int)Mathf.Floor(Random.Range(0,4))];
                int count = Random.Range(1,9);
                while(count > 0)
                {
                  float newx = Random.Range(-2.75f,2.75f);
                  float newy = Random.Range(-2.75f,2.75f);
                  GameObject tempobj = Instantiate(hidingSpotPrefabs[(int)Mathf.Floor(Random.Range(0,hidingSpotPrefabs.Count))],new Vector3(newroom.transform.position.x + newx,newroom.transform.position.y + newy,0) , transform.rotation) as GameObject;
                  tempobj.transform.parent = newroom;
                  if(newx < -1 )
                  {
                      if(newy < -1 )
                      {
                        tempobj.transform.Rotate(0,0,90);
                      }
                      else if(newy > 1 )
                      {
                        tempobj.transform.Rotate(0,0,270);
                      }
                      else
                      {
                        tempobj.transform.Rotate(0,0,180);
                      }
                  }
                  else   if(newx > 1 )
                    {
                        if(newy < -1 )
                        {
                          tempobj.transform.Rotate(0,0,90);
                        }
                        else if(newy > 1 )
                        {
                          tempobj.transform.Rotate(0,0,270);
                        }
                        else
                        {
                          tempobj.transform.Rotate(0,0,0);
                        }
                    }
                    else
                      {
                          if(newy < -1 )
                          {
                            tempobj.transform.Rotate(0,0,180);
                          }
                          else if(newy > 1 )
                          {
                            tempobj.transform.Rotate(0,0,0);
                          }
                          else
                          {
                            tempobj.transform.Rotate(0,0,0);
                          }
                      }

                  count--;
                }
                count2++;
              }
        }
    }

    public void RandomizeDoors()
    {
        int count = 0;
        while (count < doors.childCount)
        {
            if (Random.Range(0, 2.0f) > 1.5f)
            {
                gameManager.BroadCastDoorOrWall(count,false,true);

            }
            else
            { gameManager.BroadCastDoorOrWall(count, true, false); }
            count++;
        }
    }

    public void ResetHidingSpots()
    {
        int count = 0;
        while (count < roomLayOutPrefabs.Count)
        {
            roomLayOutPrefabs[count].transform.parent = layouts;
            roomLayOutPrefabs[count].active = false;
            count++;
        }
    }

    public void SetDoorOrWall(int whichspot,bool dooron, bool wallon)
    {
        doors.GetChild(whichspot).gameObject.active = dooron;
        walls.GetChild(whichspot).gameObject.active = wallon;
    }

    public void SetLayout(int whichlayout, Vector3 pos, Quaternion rot)
    {
        roomLayOutPrefabs[whichlayout].active = true;
        roomLayOutPrefabs[whichlayout].transform.position = pos;
        roomLayOutPrefabs[whichlayout].transform.rotation = rot;
        roomLayOutPrefabs[whichlayout].transform.parent = hazards;
    }

    public void RandomizeRoomSpots()
    {
        //check that it has a door connected to it
        bool validRoom = false;

        gameManager.BroadcastResetHidingSpots();
        List<GameObject> templayouts = roomLayOutPrefabs.GetRange(0,roomLayOutPrefabs.Count);//new List<GameObject>();
        while (activeRoomLayOutPrefabs.Count > 0)
        {
            roomLayOutPrefabs[activeRoomLayOutPrefabs[0]].active = false;
            activeRoomLayOutPrefabs.RemoveAt(0);

        }
        activeRoomLayOutPrefabs.Clear();
        int rnd = 0;
        int count = 0;
        while (activeRoomLayOutPrefabs.Count < rooms.childCount)
        {
             rnd = (int)Random.Range(0, roomLayOutPrefabs.Count);
            if (activeRoomLayOutPrefabs.Contains(rnd) == false)
            { activeRoomLayOutPrefabs.Add(rnd); }
        }

        foreach (Transform room in rooms)
        {
            validRoom = false;
            foreach (Transform doorobj in doors)
            {
                if (doorobj.gameObject.active == true && Vector3.Distance(doorobj.position, room.position) < 5)
                { validRoom = true; }
            }

            if (validRoom == true)
            {
                GameObject newlayout = roomLayOutPrefabs[count];
                //GameObject clone = Instantiate(newlayout, room.position, newlayout.transform.rotation);
                newlayout.transform.parent = hazards;
                newlayout.transform.position = room.position;
                if (Random.Range(0, 2.0f) < 5.0f)
                {

                    newlayout.transform.Rotate(0, 0, 270);
                }
                else if (Random.Range(0, 2.0f) < 1.0f) { newlayout.transform.Rotate(0, 0, 180); }
                else if (Random.Range(0, 2.0f) < 1.5f) { newlayout.transform.Rotate(0, 0, 90); }
                else { }
                //activeRoomLayOutPrefabs.Add(newlayout);
                //templayouts.Remove(newlayout);

                gameManager.BroadcastRoomLayout(activeRoomLayOutPrefabs[count], room.position, newlayout.transform.rotation);
            }

            count++;
        }



        gameManager.BroadcastSetHidingSpotAndDoorLists();



    }

    public void RandomizeRoomSpotsWithCorners()
    {
        //check that it has a door connected to it
        bool validRoom = false;
        bool gobackward = false;
        foreach (Transform room in rooms)
        {
            validRoom = false;
            foreach (Transform doorobj in doors)
            {
                if (doorobj.gameObject.active == true && Vector3.Distance(doorobj.position, room.position) < 5)
                { validRoom = true; }
            }

            if (validRoom == true)
            {
                float targetnum = 1.0f;
                GameObject newlayout = null;
                GameObject clone = null;
                if (gobackward == true)
                {
                    if (Random.Range(0, 2.0f) > targetnum)
                    {
                        targetnum += 0.4f;
                        newlayout = layouts.GetChild((int)Random.Range(0, layouts.childCount)).gameObject;
                        clone = Instantiate(newlayout, room.position, newlayout.transform.rotation);
                        clone.transform.Rotate(0, 0, 270); clone.transform.parent = hazards;
                    }
                    else { targetnum -= 0.2f; }
                    if (Random.Range(0, 2.0f) > targetnum)
                    {
                        targetnum += 0.4f;
                        newlayout = layouts.GetChild((int)Random.Range(0, layouts.childCount)).gameObject;
                        clone =  Instantiate(newlayout, room.position, newlayout.transform.rotation);
                        clone.transform.Rotate(0, 0, 180); clone.transform.parent = hazards;
                    }
                    else { targetnum -= 0.2f; }
                    if (Random.Range(0, 2.0f) > targetnum)
                    {
                        targetnum += 0.4f;
                        newlayout = layouts.GetChild((int)Random.Range(0, layouts.childCount)).gameObject;
                        clone = Instantiate(newlayout, room.position, newlayout.transform.rotation);
                        clone.transform.Rotate(0, 0, 90);
                        clone.transform.parent = hazards;
                    }
                    else { targetnum -= 0.2f; }
                    if (Random.Range(0, 2.0f) > targetnum)
                    {
                        targetnum += 0.4f;
                        newlayout = layouts.GetChild((int)Random.Range(0, layouts.childCount)).gameObject;
                        clone = Instantiate(newlayout, room.position, newlayout.transform.rotation);
                        clone.transform.Rotate(0, 0, 0); clone.transform.parent = hazards;
                    }
                    else { targetnum -= 0.2f; }
                }
                else
                {
                    if (Random.Range(0, 2.0f) > targetnum)
                    {
                        targetnum += 0.4f;
                        newlayout = layouts.GetChild((int)Random.Range(0, layouts.childCount)).gameObject;
                        clone = Instantiate(newlayout, room.position, newlayout.transform.rotation);
                        clone.transform.parent = hazards;
                    }
                    else { targetnum -= 0.2f; }
                    if (Random.Range(0, 2.0f) > targetnum)
                    {
                        targetnum += 0.4f;
                        newlayout = layouts.GetChild((int)Random.Range(0, layouts.childCount)).gameObject;
                        clone = Instantiate(newlayout, room.position, newlayout.transform.rotation);
                        clone.transform.Rotate(0, 0, 90); clone.transform.parent = hazards;
                    }
                    else { targetnum -= 0.2f; }
                    if (Random.Range(0, 2.0f) > targetnum)
                    {
                        targetnum += 0.4f;
                        newlayout = layouts.GetChild((int)Random.Range(0, layouts.childCount)).gameObject;
                        clone = Instantiate(newlayout, room.position, newlayout.transform.rotation);
                        clone.transform.Rotate(0, 0, 180); clone.transform.parent = hazards;
                    }
                    else { targetnum -= 0.2f; }
                    if (Random.Range(0, 2.0f) > targetnum)
                    {
                        targetnum += 0.4f;
                        newlayout = layouts.GetChild((int)Random.Range(0, layouts.childCount)).gameObject;
                        clone = Instantiate(newlayout, room.position, newlayout.transform.rotation);
                        clone.transform.Rotate(0, 0, 270); clone.transform.parent = hazards;
                    }
                    else { targetnum -= 0.2f; }
                }
                gobackward = !gobackward;
                
                



            }
        }
    }

    public void RandomizeRoomSpots2()
    {
        //check that it has a door connected to it
        bool validRoom = false;
        foreach (Transform room in rooms)
        {
            validRoom = false;
            foreach (Transform doorobj in doors)
            {
                if (doorobj.gameObject.active == true && Vector3.Distance(doorobj.position, room.position) < 5)
                { validRoom = true; }
            }

            int count = Random.Range(1, 9);
            if (validRoom == true)
            {
                while (count > 0)
                {
                    float newx = Random.Range(-2.75f, 2.75f);
                    float newy = Random.Range(-2.75f, 2.75f);
                    GameObject tempobj = Instantiate(hidingSpotPrefabs[(int)Mathf.Floor(Random.Range(0, hidingSpotPrefabs.Count))], new Vector3(room.transform.position.x + newx, room.transform.position.y + newy, 0), transform.rotation) as GameObject;
                    tempobj.transform.parent = hazards;
                    if (newx < -1)
                    {
                        if (newy < -1)
                        {
                            tempobj.transform.Rotate(0, 0, 90);
                        }
                        else if (newy > 1)
                        {
                            tempobj.transform.Rotate(0, 0, 270);
                        }
                        else
                        {
                            tempobj.transform.Rotate(0, 0, 180);
                        }
                    }
                    else if (newx > 1)
                    {
                        if (newy < -1)
                        {
                            tempobj.transform.Rotate(0, 0, 90);
                        }
                        else if (newy > 1)
                        {
                            tempobj.transform.Rotate(0, 0, 270);
                        }
                        else
                        {
                            tempobj.transform.Rotate(0, 0, 0);
                        }
                    }
                    else
                    {
                        if (newy < -1)
                        {
                            tempobj.transform.Rotate(0, 0, 180);
                        }
                        else if (newy > 1)
                        {
                            tempobj.transform.Rotate(0, 0, 0);
                        }
                        else
                        {
                            tempobj.transform.Rotate(0, 0, 0);
                        }
                    }

                    count--;
                }
            }
        }
    }
    public Transform MakeNewRoom()
    {
        GameObject newroom = Instantiate(roomPrefab,transform.position,transform.rotation) as GameObject;
        int roomLayout = (int)Mathf.Floor(Random.Range(0,roomDoorLayouts.Count));
        newroom.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = roomDoorLayouts[roomLayout];
        westCollider = newroom.transform.GetChild(1).GetChild(2).gameObject;
        northCollider = newroom.transform.GetChild(1).GetChild(3).gameObject;
        eastCollider = newroom.transform.GetChild(1).GetChild(0).gameObject;
        southCollider = newroom.transform.GetChild(1).GetChild(1).gameObject;
        SetCenterCollidersBasedOnRoomPattern(roomLayout);
        return newroom.transform;
    }

    public void SetCenterCollidersBasedOnRoomPattern(int roomLayout)
    {
      westCollider.active = true;
      eastCollider.active = true;
      northCollider.active = true;
      southCollider.active = true;
        switch(roomLayout)
        {
          case 0:
          northCollider.active = false;
          break;
          case 1:
          northCollider.active = false;
          southCollider.active = false;
          break;
          case 2:
          northCollider.active = false;
          eastCollider.active = false;
          break;
          case 3:
          northCollider.active = false;
          southCollider.active = false;
          eastCollider.active = false;
          break;
          case 4:
          northCollider.active = false;
          southCollider.active = false;
          eastCollider.active = false;
          westCollider.active = false;
          break;
          default:

          break;
        }
    }

}
