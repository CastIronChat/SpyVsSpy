using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HazardManager : MonoBehaviour
{
  public GameManager gameManager;
  public Transform hazardsParent;
  public List<GameObject> hazards;
    // Start is called before the first frame update
    void Start()
    {
      SetHazardList();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ActivateHazard(int whichHazard)
    {

      if(hazards.Count > whichHazard)
      {
        print("disable hazard");
        hazards[whichHazard].GetComponent<Hazard>().Activate();
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
