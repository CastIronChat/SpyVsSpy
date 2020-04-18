using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TurnManager :  Photon.MonoBehaviour
{
    public Transform car;
    public Vector3 startPosition,lastPosition;
    public float totalDistance,totalDisplacement;
    public Text distanceText,displacementText;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      TrackDistance();
    }

    [PunRPC]
    public void NewTurn()
    {
      startPosition = car.position;
      lastPosition = car.position;
      totalDistance = 0;
      totalDisplacement = 0;

    }
    public void TrackDistance()
    {
      //NOTE: think about total distance traveled and distance from start position
      totalDistance += Vector3.Distance(car.position,lastPosition);
      lastPosition = car.position;
      totalDisplacement = Vector3.Distance(car.position,startPosition);

      distanceText.text =  Mathf.Round(totalDistance).ToString();
      displacementText.text = Mathf.Round(totalDisplacement).ToString();
    }
}
