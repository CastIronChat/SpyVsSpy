using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TurnManager :  Photon.MonoBehaviour
{
  public GameManager gameManager;
    public Transform car,checkpoint;
    public bool switchingPlayers;
    public Vector3 startPosition,lastPosition;
    public float safeSpotTimer, timer,turnTime = 10.0f;
    public float totalDistance,totalDisplacement;
    public Text distanceText,displacementText;
    public float carReturnSpeed = 5.0f,checkpointDist = 5.0f;
    public int currentCheckpoint,checkPointsCrossed,checkPointsNeededPerRound;
    public Quaternion lastSafeRotation,secondLastSafeRotation;
    public Vector3 lastSafeSpot,secondLastSafeSpot;
    // Start is called before the first frame update
    void Start()
    {
      secondLastSafeSpot = car.position;
      secondLastSafeRotation = car.rotation;
      lastSafeRotation = car.rotation;
      lastSafeSpot = car.position;
    }

    // Update is called once per frame
    void Update()
    {

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

    public void TurnActive()
    {
      safeSpotTimer += Time.deltaTime;
      if(safeSpotTimer > 2.0f)
      {
          safeSpotTimer = 0;
          secondLastSafeSpot = lastSafeSpot;
          secondLastSafeRotation = lastSafeRotation;
          lastSafeRotation = car.rotation;
          lastSafeSpot = car.position;
      }
      if(Vector3.Distance(car.position,gameManager.currentTrack.checkpoints.GetChild(currentCheckpoint).position) < checkpointDist)
      {NextCheckPoint();}
      TrackDistance();
    }

    public void NextCheckPoint()
    {
      int lastcheckpoint = checkPointsCrossed;
      checkPointsCrossed++;
      gameManager.currentTrack.checkpoints.GetChild(currentCheckpoint).gameObject.active = false;
        currentCheckpoint++;
        if(currentCheckpoint > gameManager.currentTrack.checkpoints.childCount)
        {currentCheckpoint = 0;}
        gameManager.currentTrack.checkpoints.GetChild(currentCheckpoint).gameObject.active = true;
        Vector3 lastcheckpointpos = gameManager.currentTrack.checkpoints.GetChild(lastcheckpoint).position;
        gameManager.currentTrack.checkpoints.GetChild(currentCheckpoint).LookAt(new Vector3(lastcheckpointpos.x,gameManager.currentTrack.checkpoints.GetChild(currentCheckpoint).position.y,lastcheckpointpos.z));
    }

    public bool ReturnCarToSafeSpot()
    {
      if(car.position != secondLastSafeSpot)
      {
        car.rotation = Quaternion.Slerp(car.rotation, secondLastSafeRotation, carReturnSpeed * Time.deltaTime);
        car.position = Vector3.MoveTowards(car.position, secondLastSafeSpot, carReturnSpeed  * Time.deltaTime);
        return false;
      }else{    return true;}

    }
    public void HitSomething( float magnitude)
    {
      switchingPlayers = true;
    }


    [PunRPC]
    public void NewTurn()
    {
      checkPointsCrossed = 0;
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


    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {

        }
        else
        {

        }
    }
}
