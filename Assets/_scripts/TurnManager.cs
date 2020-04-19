using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TurnManager :  Photon.MonoBehaviour
{
  public GameManager gameManager;
    public Transform car;
    public bool switchingPlayers;
    public Vector3 startPosition,lastPosition;
    public float timer,turnTime = 10.0f;
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
      // if(switchingPlayers == false)
      // {
      //   timer -= Time.deltaTime;
      //   if(timer > 0)
      //   {TrackDistance();}
      //   else{
      //     switchingPlayers = true;
      //     // gameManager.SwitchTurn();
      //   }

      // }

    }

    public void HitSomething( float magnitude)
    {
      switchingPlayers = true;
    }


    [PunRPC]
    public void NewTurn()
    {
      startPosition = car.position;
      lastPosition = car.position;
      totalDistance = 0;
      totalDisplacement = 0;
      timer = turnTime;
        switchingPlayers = false;
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
