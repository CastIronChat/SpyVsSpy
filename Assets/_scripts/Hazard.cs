using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
  public GameManager gameManager;
  public bool setInManagerList;
  public int spotInHazardList = -1;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetSpotInList(int newplace, GameManager newGM)
    {
      gameManager = newGM;
      setInManagerList = true;
      spotInHazardList = newplace;
    }

    public void AttemptToActivate()
    {
      gameManager.AttemptToActivateHazard(spotInHazardList);
    }

    public void OnMouseDown()
    {
      print("clicked " + spotInHazardList);
      AttemptToActivate();
    }
}
