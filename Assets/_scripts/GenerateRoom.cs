using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRoom : MonoBehaviour
{
  public SpriteRenderer wallSprite;
    public List<Sprite> roomDoorLayouts,floorPattern;
    public List<GameObject> hidingSpotPrefabs;
    public GameObject roomPrefab,westCollider,eastCollider,northCollider,southCollider; //for when theyre isnt a door on that side
    public Transform newroomparent;
    public float roomradius = 2.75f;
    public int yCount = 0,xCount = 0;
    void Start()
    {

    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
          int count2 = 0;
          while(count2 < 20){
                yCount++;
                if(yCount > 4){yCount = 0; xCount++;}
                transform.position = new Vector3((7 * xCount), (yCount * 7));
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
