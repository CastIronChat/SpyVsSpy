using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : Photon.MonoBehaviour
{
  public PlayerManager playerManager;
  public TurnManager turnManager;
  public HazardManager hazardManager;
  public CarControls car;
  public GameObject playerPrefab;
  public GameObject scoreBoard;
  public int activePlayers;
  public int currentPlayer = -1,currentTurn = -1;//turn is 0-x and player is their photonid
  public Renderer myRenderer;
  public List<Material> colors;
  public Transform colorBlindShapes;
  public Transform idleplayerManager;

  public int startingLives = 3;
  public int localPlayer; //set to the photonid of the local player via the player script for checking if objects can be activated
  public bool roundActive,switchingPlayers;
  public int partOfTurn; //0 inactive, 1 active, 2 changing players

    public void Awake()
    {
        if (!PhotonNetwork.connected)
        {

        }
        // GameObject clone = PhotonNetwork.Instantiate(this.playerPrefab.name, transform.position, Quaternion.identity, 0);
        // clone.GetComponent<Player>().gameManager = GetComponent<GameManager>();
    }
    [PunRPC]
    public void NextTurn(int setTurn)
    {
        // currentPlayer = playerManager.transform.GetChild(setTurn).GetComponent<Player>().playerNum;


        foreach(Transform child in colorBlindShapes)
        {child.gameObject.active = false;}
        colorBlindShapes.GetChild(setTurn).gameObject.active = true;
        // colorBlindShapes.GetChild(setTurn).GetComponent<Renderer>().material = colors[setTurn]
        myRenderer.material = colors[setTurn];
        foreach(Transform go in playerManager.transform)
        {
          // go.GetComponent<Player>().myScoreCard.transform.GetChild(1).GetComponent<Text>().text = go.GetComponent<Player>().name + " : ";
          // go.GetComponent<Player>().myScoreCard.transform.GetChild(2).GetComponent<Text>().text = go.GetComponent<Player>().score.ToString();

          if(go.GetComponent<Player>().numberInList == setTurn)
          {go.GetComponent<Player>().StartMyTurn(setTurn);}

        }
        // turnManager.NewTurn();
        // scoreBoard.transform.GetChild(playercount).GetChild(1).GetComponent<Text>().text = nextPlayerInOrder.GetComponent<Player>().name + " : ";
        // scoreBoard.transform.GetChild(playercount).GetChild(2).GetComponent<Text>().text = nextPlayerInOrder.GetComponent<Player>().score.ToString();
        // PopulateScoreBoard(1,"");
    }
    [PunRPC]
    public void PlayerJoinGame(int newPlayer,string newname  )
    {
        print("new player Number: " + newPlayer);
        PopulateScoreBoard(newPlayer,newname);



    }

    public void PopulateScoreBoard(int newPlayer,string newname )
    {
      int cardcount = 0;
      int playercount = 0;

      Transform nextPlayerInOrder = playerManager.transform.GetChild(0);
      if(nextPlayerInOrder.GetComponent<Player>().playerNum <= 0)
      {
        // nextPlayerInOrder.GetComponent<Player>().lives = startingLives;

        nextPlayerInOrder.GetComponent<Player>().playerNum = nextPlayerInOrder.GetComponent<PhotonView>().viewID;
      }
      int lowestPlayerNum = 99999;
      int lastPlayerNum = -1;

        while(playercount < playerManager.transform.childCount)
        {


            foreach(Transform go in playerManager.transform)
            {
              //So the new players has their name when they join
              if(go.GetComponent<Player>().name.Length <= 0){
                go.GetComponent<Player>().name = newname;

              }
              if(go.GetComponent<Player>().playerNum <= 0){
                go.GetComponent<Player>().playerNum = go.GetComponent<PhotonView>().viewID;

              }
              if(go.GetComponent<Player>().playerNum > lastPlayerNum && go.GetComponent<Player>().playerNum < lowestPlayerNum )
              {
                nextPlayerInOrder = go;
                lowestPlayerNum = go.GetComponent<Player>().playerNum;
              }

            }
            lastPlayerNum =  lowestPlayerNum;
            lowestPlayerNum = 99999;

            scoreBoard.transform.GetChild(playercount).gameObject.active = true;
            scoreBoard.transform.GetChild(playercount).GetChild(1).GetComponent<Text>().text = nextPlayerInOrder.GetComponent<Player>().name + " : ";
            scoreBoard.transform.GetChild(playercount).GetChild(2).GetComponent<Text>().text = nextPlayerInOrder.GetComponent<Player>().score.ToString();

            nextPlayerInOrder.GetComponent<Player>().myScoreCard = scoreBoard.transform.GetChild(playercount).gameObject;
            nextPlayerInOrder.parent = playerManager.transform;
            if( PhotonNetwork.isMasterClient  )
            {
      nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer, nextPlayerInOrder.GetComponent<Player>().lives );
                nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "SetNumberInList", PhotonTargets.AllBufferedViaServer, playercount );
            }



            scoreBoard.transform.GetChild(playercount).GetChild(5).GetComponent<RawImage>().color = colors[playercount].color;
            playercount++;
            if(playercount >= scoreBoard.transform.childCount){return;}
       }
       while(playercount < scoreBoard.transform.childCount)
       {
         scoreBoard.transform.GetChild(playercount).gameObject.active = false;
           playercount++;
       }

    }
     void Update()
    {

            if(partOfTurn == 2)
            {

              if(turnManager.ReturnCarToSafeSpot() == true)
              {
                partOfTurn = 1;
                switchingPlayers = false;
                StartTurn();
              }else
              {}

            }
            else if(partOfTurn == 1)
            {
              if(car.GetComponent<PhotonView>().isMine){  car.DriveCar();}

              turnManager.TurnActive();

            }

        if( PhotonNetwork.isMasterClient   && Input.GetKeyDown(KeyCode.Space)   )
        {
          // turnManager.timer = 10;
          // turnManager.switchingPlayers = true;

          if(partOfTurn == 0){
            roundActive = true;
            partOfTurn = 1;
            StartTurn();
            // this.GetComponent<PhotonView>().RPC( "StartRound", PhotonTargets.AllBufferedViaServer );
          }else{  EndTurn(true);}


        }

        if( PhotonNetwork.isMasterClient   && Input.GetKeyDown(KeyCode.P)   )
        {
          // turnManager.timer = 10;
          // turnManager.switchingPlayers = true;
            // roundActive = false;


        }

    }
    [PunRPC]
    public void StartRound()
    {
      roundActive = true;
      int playercount = 0;
      foreach(Transform go in idleplayerManager)
      {go.transform.parent = playerManager.transform;}

          currentTurn = 0;
          myRenderer.material = colors[currentTurn];
          foreach(Transform child in colorBlindShapes)
          {child.gameObject.active = false;}
          colorBlindShapes.GetChild(0).gameObject.active = true;


          Transform nextPlayerInOrder = playerManager.transform.GetChild(0);
          if(nextPlayerInOrder.GetComponent<Player>().playerNum <= 0)
          {
            // nextPlayerInOrder.GetComponent<Player>().lives = startingLives;

            nextPlayerInOrder.GetComponent<Player>().playerNum = nextPlayerInOrder.GetComponent<PhotonView>().viewID;
          }
          int lowestPlayerNum = 99999;
          int lastPlayerNum = -1;

            while(playercount < playerManager.transform.childCount)
            {


                foreach(Transform go in playerManager.transform)
                {
                  //So the new players has their name when they join

                  if(go.GetComponent<Player>().playerNum <= 0){
                    go.GetComponent<Player>().playerNum = go.GetComponent<PhotonView>().viewID;

                  }
                  if(go.GetComponent<Player>().playerNum > lastPlayerNum && go.GetComponent<Player>().playerNum < lowestPlayerNum )
                  {
                    nextPlayerInOrder = go;
                    lowestPlayerNum = go.GetComponent<Player>().playerNum;
                  }

                }
                lastPlayerNum =  lowestPlayerNum;
                lowestPlayerNum = 99999;

                scoreBoard.transform.GetChild(playercount).gameObject.active = true;
                scoreBoard.transform.GetChild(playercount).GetChild(1).GetComponent<Text>().text = nextPlayerInOrder.GetComponent<Player>().name + " : ";
                scoreBoard.transform.GetChild(playercount).GetChild(2).GetComponent<Text>().text = nextPlayerInOrder.GetComponent<Player>().score.ToString();

                nextPlayerInOrder.GetComponent<Player>().myScoreCard = scoreBoard.transform.GetChild(playercount).gameObject;
                nextPlayerInOrder.parent = playerManager.transform;
                if( PhotonNetwork.isMasterClient  )
                {}
                    nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer, nextPlayerInOrder.GetComponent<Player>().lives );
                    nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "SetNumberInList", PhotonTargets.AllBufferedViaServer, playercount );




                scoreBoard.transform.GetChild(playercount).GetChild(5).GetComponent<RawImage>().color = colors[playercount].color;
                playercount++;
                if(playercount >= scoreBoard.transform.childCount){return;}
           }
           while(playercount < scoreBoard.transform.childCount)
           {
             scoreBoard.transform.GetChild(playercount).gameObject.active = false;
               playercount++;
           }

           if( PhotonNetwork.isMasterClient  )
           {}
           turnManager.GetComponent<PhotonView>().photonView.RPC( "NewTurn", PhotonTargets.AllBufferedViaServer );
           playerManager.transform.GetChild(0).GetComponent<Player>().StartMyTurn(0);

    }
    public void EndTurn(bool returnCarToSafeSpot)
    {
      roundActive = false;
      partOfTurn = 2;
      switchingPlayers = returnCarToSafeSpot;

      //take the car and move it to the safe location

      car.photonView.RequestOwnership();
      car.GetComponent<Rigidbody>().isKinematic = true;
      foreach(Transform go in playerManager.transform)
      {


            if(go.GetComponent<Player>().numberInList == currentTurn)
            {
                //if the car is returning to a safe spot it means the player crashed and does not score
                if(returnCarToSafeSpot == false){
                  go.GetComponent<PhotonView>().RPC( "UpdateScore", PhotonTargets.AllBufferedViaServer,
                  go.GetComponent<Player>().score + (int)(turnManager.totalDistance * turnManager.totalDisplacement * 0.1f) );
                }
                // not scoring gives the player more power/money
                else
                {
                      go.GetComponent<PhotonView>().RPC( "UpdatePower", PhotonTargets.AllBufferedViaServer,
                      go.GetComponent<Player>().money + (int)(turnManager.totalDistance * turnManager.totalDisplacement * 0.1f) );
                }
          }
      }

      if(returnCarToSafeSpot == false){partOfTurn = 1;}

    }
    public void StartTurn( )
    {
      roundActive = true;
      switchingPlayers = false;
      //take the car and move it to the safe location

      // car.photonView.RequestOwnership();
      car.GetComponent<Rigidbody>().isKinematic = false;


          currentTurn = currentTurn - 1;
          if(currentTurn < 0){currentTurn = playerManager.transform.childCount - 1;}
          myRenderer.material = colors[currentTurn];
          this.photonView.RPC( "NextTurn", PhotonTargets.AllBufferedViaServer, currentTurn );

          turnManager.GetComponent<PhotonView>().photonView.RPC( "NewTurn", PhotonTargets.AllBufferedViaServer );
            roundActive = true;

    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

          if (stream.isWriting)
          {

              stream.SendNext(partOfTurn);
              // stream.SendNext(roundActive);
          }
          else
          {

            partOfTurn = (int)stream.ReceiveNext();
            // roundActive = (bool)stream.ReceiveNext();


          }

    }

    public void AttemptToActivateHazard(int hazardToActivate)
    {
              this.photonView.RPC( "AttemptToActivateHazardRPC", PhotonTargets.AllBufferedViaServer, localPlayer, hazardToActivate );
    }

    [PunRPC]
    public void AttemptToActivateHazardRPC( int whichPlayer, int hazardToActivate )
    {
      if(PhotonNetwork.isMasterClient)
      {
          foreach(Transform go in playerManager.transform)
          {
              if(go.GetComponent<Player>().playerNum == whichPlayer && go.GetComponent<Player>().money > 0)
              {

                  this.photonView.RPC( "ActivateHazardRPC", PhotonTargets.AllBufferedViaServer, whichPlayer, hazardToActivate );
              }

          }

      }


    }

    [PunRPC]
    public void ActivateHazardRPC( int whichPlayer, int hazardToActivate )
    {
      hazardManager.ActivateHazard(hazardToActivate);

          foreach(Transform go in playerManager.transform)
          {
              if(go.GetComponent<Player>().playerNum == whichPlayer )
              {go.GetComponent<Player>().money -= 1;}

          }

    }

    [PunRPC]
    public void HitSomething( float magnitude)
    {
      print("hit something: " + magnitude + " " + PhotonNetwork.isMasterClient);
      if( partOfTurn == 1 && PhotonNetwork.isMasterClient  && magnitude > 5 )
      {
            foreach(Transform go in playerManager.transform)
            {
            if(go.GetComponent<Player>().numberInList == currentTurn)
            {go.GetComponent<PhotonView>().RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer, go.GetComponent<Player>().lives - 1);}

            }
            EndTurn(true);
        }

    }

    // public void OnGUI()
    // {
    //     if (GUILayout.Button("Return to Lobby"))
    //     {
    //         PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
    //     }
    // }

    public void OnMasterClientSwitched(PhotonPlayer player)
    {
        Debug.Log("XXXXXXOnMasterClientSwitched: " + player);


    }

    public void OnLeftRoom()
    {
        Debug.Log("XXXXXXOnLeftRoom (local)");

        // back to main menu
        // SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("XXXXOnDisconnectedFromPhoton");

        // back to main menu
        // SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("XXXXXOnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("XXXXXXOnPhotonPlayerConnected: " + player);
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("XXXXXXXXXOnPlayerDisconneced: " + player);
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("XXXXXXXXXXOnFailedToConnectToPhoton");

        // back to main menu
        // SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
    }
}
