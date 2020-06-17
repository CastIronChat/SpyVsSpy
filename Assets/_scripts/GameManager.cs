using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Photon.MonoBehaviour
{
  public PlayerManager playerManager;
  public RoundManager roundManager;
  public HazardManager hazardManager;



  public GameObject playerPrefab;
  public GameObject scoreBoard;
  public int activePlayers;
  public int currentPlayerPhotonID,currentPlayer = -1,currentTurn = -1,currentCheckpoint;//turn is 0-x and player is their photonid
  public Renderer myRenderer;
  public List<Material> colors;
  public Transform colorBlindShapes;
  public Transform idleplayerManager;


    public void Awake()
    {
        if (!PhotonNetwork.connected)
        {

        }
        if (PhotonNetwork.isMasterClient)
        {

        }

        // GameObject clone = PhotonNetwork.Instantiate(this.playerPrefab.name, transform.position, Quaternion.identity, 0);
        // clone.GetComponent<Player>().gameManager = GetComponent<GameManager>();
    }


    [PunRPC]
    public void PlayerJoinGame(int newPlayer,string newname  )
    {
      int count = 0;

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

        nextPlayerInOrder.GetComponent<Player>().playerNum = nextPlayerInOrder.GetComponent<PhotonView>().ownerId;
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
                go.GetComponent<Player>().playerNum = go.GetComponent<PhotonView>().ownerId;

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

            nextPlayerInOrder.GetComponent<Player>().myScoreCard = scoreBoard.transform.GetChild(playercount).gameObject;
            // nextPlayerInOrder.parent = playerManager.transform;
            if( PhotonNetwork.isMasterClient  )
            {
              nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer, nextPlayerInOrder.GetComponent<Player>().lives );
              nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "UpdateScore", PhotonTargets.AllBufferedViaServer, nextPlayerInOrder.GetComponent<Player>().score );
              nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "UpdatePower", PhotonTargets.AllBufferedViaServer, nextPlayerInOrder.GetComponent<Player>().money );
                nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "SetNumberInList", PhotonTargets.AllBufferedViaServer, playercount );
            }


            scoreBoard.transform.GetChild(playercount).GetChild(5).GetComponent<RawImage>().color = nextPlayerInOrder.GetComponent<SpriteRenderer>().color = nextPlayerInOrder.GetComponent<Player>().colors[nextPlayerInOrder.GetComponent<Player>().photonView.ownerId] ;

            // scoreBoard.transform.GetChild(playercount).GetChild(5).GetComponent<RawImage>().color = colors[playercount].color;
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




        if( PhotonNetwork.isMasterClient   && Input.GetKeyDown(KeyCode.RightShift)   )
        {


        }

        if( PhotonNetwork.isMasterClient )
        {


        }


    }


    public void VoteForNewRoundType(int rndtype, int fromplayer)
    {
      photonView.RPC( "rpcVoteForNewRoundType", PhotonTargets.AllBufferedViaServer, rndtype,fromplayer );
    }

    [PunRPC]
    public void rpcVoteForNewRoundType(int rndtype, int fromplayer)
    {
      if(playerManager.transform.childCount > 1)
      {
        StartRound();
      }

    }

    [PunRPC]
    public void StartRound()
    {
      roundManager.DisableUi();

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

            nextPlayerInOrder.GetComponent<Player>().playerNum = nextPlayerInOrder.GetComponent<PhotonView>().ownerId;
          }
          int lowestPlayerNum = 99999;
          int lastPlayerNum = -1;

            while(playercount < playerManager.transform.childCount)
            {


                foreach(Transform go in playerManager.transform)
                {
                  //So the new players has their name when they join

                  if(go.GetComponent<Player>().playerNum <= 0){
                    go.GetComponent<Player>().playerNum = go.GetComponent<PhotonView>().ownerId;

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



                scoreBoard.transform.GetChild(playercount).GetChild(5).GetComponent<RawImage>().color = nextPlayerInOrder.GetComponent<SpriteRenderer>().color = nextPlayerInOrder.GetComponent<Player>().colors[nextPlayerInOrder.GetComponent<Player>().photonView.ownerId] ;
                if(playercount >= scoreBoard.transform.childCount){return;}
           }
           while(playercount < scoreBoard.transform.childCount)
           {
             scoreBoard.transform.GetChild(playercount).gameObject.active = false;
               playercount++;
           }



           // turnManager.GetComponent<PhotonView>().photonView.RPC( "NewTurn", PhotonTargets.AllBufferedViaServer );
           // playerManager.transform.GetChild(0).GetComponent<Player>().StartMyTurn(0);
           if( PhotonNetwork.isMasterClient  )
           {this.photonView.RPC( "NextTurn", PhotonTargets.AllViaServer, 0 );}




    }



    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

          if (stream.isWriting)
          {

              // stream.SendNext(1);
              // stream.SendNext(roundActive);
          }
          else
          {

            // partOfTurn = (int)stream.ReceiveNext();
            // roundActive = (bool)stream.ReceiveNext();


          }

    }

    public void AttemptToActivateHazard(int hazardToActivate)
    {
              this.photonView.RPC( "AttemptToActivateHazardRPC", PhotonTargets.AllBufferedViaServer, 1, hazardToActivate );
    }

    [PunRPC]
    public void AttemptToActivateHazardRPC( int whichPlayer, int hazardToActivate )
    {
      if(PhotonNetwork.isMasterClient)
      {
          foreach(Transform go in playerManager.transform)
          {
              if(go.GetComponent<Player>().playerNum == whichPlayer && go.GetComponent<Player>().money > hazardManager.GetHazardCost(hazardToActivate))
              {
                go.GetComponent<PhotonView>().RPC( "UpdatePower", PhotonTargets.AllBufferedViaServer, go.GetComponent<Player>().lives - hazardManager.GetHazardCost(hazardToActivate) );
                this.photonView.RPC( "ActivateHazardRPC", PhotonTargets.All, whichPlayer, hazardToActivate );
              }

          }

      }


    }

    //make sure this RPC is not buffered so that traps dont go off when players join
    [PunRPC]
    public void ActivateHazardRPC( int whichPlayer, int hazardToActivate )
    {
      hazardManager.ActivateHazard(hazardToActivate);


    }

    [PunRPC]
    public void HitSomething( float magnitude)
    {
        print("hit something: " + magnitude + " " + PhotonNetwork.isMasterClient);
        if(  PhotonNetwork.isMasterClient  )
        {
              foreach(Transform go in playerManager.transform)
              {
                  if(go.GetComponent<Player>().numberInList == currentTurn)
                  {
                    go.GetComponent<PhotonView>().RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer, go.GetComponent<Player>().lives - 1);
                  }

              }
        }

    }


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

        // Debug.Log("XXXXXX  OnPhotonPlayerConnected: " + player);
        // ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        // hash.Add("lives", player.ID);
      // PhotonNetwork.playerList[player.ID - 1].SetCustomProperties(hash);

        // player.SetCustomProperties(hash);
        //   player.UpdateCustomProperty("score", player.ID);
        // player.UpdateCustomProperty("lives",player.ID);
        // PhotonNetwork.player.UpdateCustomProperty("Deaths", 1);
        // GameObject clone = PhotonNetwork.Instantiate(this.playerPrefab.name, transform.position, Quaternion.identity, 0) ;

    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPlayerDisconneced: " + player);
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("OnFailedToConnectToPhoton");

        // back to main menu
        // SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
    }
}
