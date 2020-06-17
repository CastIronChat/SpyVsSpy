using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager : MonoBehaviour
{
  public GameManager gameManager;
  public ScrollingText scrollingText;
  public Transform hazardsParent;
  public List<GameObject> hazards,buttonObjectDisplays;
  public List<int> hazardsListeningForButtonPress;
  private int hazardButtonTracking = 0;
    // Start is called before the first frame update
    void Start()
    {
      SetHazardList();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void HazardRequestButton(Hazard visibleHazard)
    {
      if(hazardsListeningForButtonPress.Count < buttonObjectDisplays.Count){hazardsListeningForButtonPress.Add(-1);}
      if(hazardButtonTracking == 0){visibleHazard.buttonToListenFor = "A";}
      else if(hazardButtonTracking == 1){visibleHazard.buttonToListenFor = "B";}
      else if(hazardButtonTracking == 2){visibleHazard.buttonToListenFor = "X";}
      else if(hazardButtonTracking == 3){visibleHazard.buttonToListenFor = "Y";}
      else{}
      if(hazardsListeningForButtonPress.Count > 0 && hazardsListeningForButtonPress[hazardButtonTracking] != -1)
      {
        hazards[hazardsListeningForButtonPress[hazardButtonTracking]].GetComponent<Hazard>().StopListening();
      }
      hazardsListeningForButtonPress[hazardButtonTracking] = visibleHazard.spotInHazardList;
      visibleHazard.buttonIndicator = buttonObjectDisplays[hazardButtonTracking];
      buttonObjectDisplays[hazardButtonTracking].transform.position = visibleHazard.transform.position;
buttonObjectDisplays[hazardButtonTracking].active = true;
      hazardButtonTracking++;

      if(hazardButtonTracking >= buttonObjectDisplays.Count)
      {hazardButtonTracking = 0;}
    }

    public int GetHazardCost(int whichHazard)
    {
      if(whichHazard < hazards.Count && whichHazard >= 0)
      {
        return hazards[whichHazard].GetComponent<Hazard>().costToActivate;
      }else{return 0;}

    }
    public void ActivateHazard(int whichHazard)
    {

      if(hazards.Count > whichHazard)
      {
        hazards[whichHazard].GetComponent<Hazard>().Activate();
        if(hazards[whichHazard].GetComponent<Hazard>().isOn == true)
        {  scrollingText.NewLine(hazards[whichHazard].transform.name + " Activated");}
        else{  scrollingText.NewLine(hazards[whichHazard].transform.name + " Disabled");}

      }
    }

    //so that each hazard has a unique identifier use their distance to sort the list
    public void SetHazardList()
    {

      while(hazards.Count < hazardsParent.childCount)
      {
        Transform closestHazard = hazardsParent.GetChild(0);
        float dist = Vector3.Distance(closestHazard.position,transform.position);
        foreach(Transform go in hazardsParent)
        {
          if(go.GetComponent<Hazard>().setInManagerList == false && (closestHazard.GetComponent<Hazard>().setInManagerList == true || Vector3.Distance(go.position,transform.position) < dist))
          {
              closestHazard = go;
              dist = Vector3.Distance(go.position,transform.position);
          }
        }


        closestHazard.GetComponent<Hazard>().SetSpotInList(  hazards.Count, gameManager);
        closestHazard.GetComponent<Hazard>().Deactivate();
        hazards.Add(closestHazard.gameObject);
      }

    }


}
