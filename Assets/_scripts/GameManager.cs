using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Photon.MonoBehaviour
{
    public PlayerManager playerManager;
    public HidingspotManager hidingSpotManager; //Track all the hiding spots in a single place rather than have each hiding spot handle itself
    public ScrollingText scrollingText;
    public RoundManager roundManager;
    public GameConstants gameConstants;


    public GameObject playerPrefab;
    public GameObject scoreBoard,startbutton;
    public int activePlayers;
    public Renderer myRenderer;
    public List<Material> colors;
    public Transform colorBlindShapes,rooms;
    public Transform idleplayerManager;


    public void Awake()
    {
        if ( !PhotonNetwork.connected )
        {

        }
        if ( PhotonNetwork.isMasterClient )
        {
                startbutton.active = true;
        }

        // GameObject clone = PhotonNetwork.Instantiate(this.playerPrefab.name, transform.position, Quaternion.identity, 0);
        // clone.GetComponent<Player>().gameManager = GetComponent<GameManager>();
    }


    [PunRPC]
    public void PlayerJoinGame(int newPlayer, string newname)
    {
        print( "new player Number: " + newPlayer );
        PopulateScoreBoard( newPlayer, newname );

    }


    void Update()
    {




        if ( PhotonNetwork.isMasterClient && Input.GetKeyDown( KeyCode.RightShift ) )
        {


        }

        if ( PhotonNetwork.isMasterClient )
        {


        }


    }


    public void VoteForNewRoundType(int rndtype, int fromplayer)
    {
        photonView.RPC( "rpcVoteForNewRoundType", PhotonTargets.AllBufferedViaServer, rndtype, fromplayer );
    }


    public void TryToStartRound(int rndtype)
    {
        if ( PhotonNetwork.isMasterClient )
        {
          photonView.RPC( "StartRound", PhotonTargets.AllBufferedViaServer );
        }
    }


    [PunRPC]
    public void rpcVoteForNewRoundType(int rndtype, int fromplayer)
    {
        if ( playerManager.transform.childCount > 1 )
        {
            StartRound();
        }

    }

    [PunRPC]
    public void StartRound()
    {

        startbutton.active = false;

        int playercount = 0;
        foreach ( Transform go in idleplayerManager )
        { go.transform.parent = playerManager.transform; }




        Transform nextPlayerInOrder = playerManager.transform.GetChild( 0 );
        if ( nextPlayerInOrder.GetComponent<Player>().playerNum <= 0 )
        {
            // nextPlayerInOrder.GetComponent<Player>().lives = startingLives;

            nextPlayerInOrder.GetComponent<Player>().playerNum = nextPlayerInOrder.GetComponent<PhotonView>().ownerId;
        }
        int lowestPlayerNum = 99999;
        int lastPlayerNum = -1;

        //sort players by their ownerid so that the transform has the same child order for all players
        while ( playercount < playerManager.transform.childCount )
        {


            foreach ( Transform go in playerManager.transform )
            {

                if ( go.GetComponent<PhotonView>().ownerId > lastPlayerNum && go.GetComponent<PhotonView>().ownerId < lowestPlayerNum )
                {
                    nextPlayerInOrder = go;
                    lowestPlayerNum = go.GetComponent<Player>().playerNum;
                }

            }
            lastPlayerNum = lowestPlayerNum;
            lowestPlayerNum = 99999;

            scoreBoard.transform.GetChild( playercount ).gameObject.active = true;
            scoreBoard.transform.GetChild( playercount ).GetChild( 1 ).GetComponent<Text>().text = nextPlayerInOrder.GetComponent<Player>().name + " : ";
            scoreBoard.transform.GetChild( playercount ).GetChild( 2 ).GetComponent<Text>().text = nextPlayerInOrder.GetComponent<Player>().score.ToString();

            nextPlayerInOrder.GetComponent<Player>().myScoreCard = scoreBoard.transform.GetChild( playercount ).gameObject;
            nextPlayerInOrder.parent = playerManager.transform;
            if ( PhotonNetwork.isMasterClient )
            {
                int addtraps = gameConstants.startingTraps;
                while(addtraps > 0)
                {
                  nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "AddorRemoveTrap", PhotonTargets.AllBufferedViaServer, addtraps,1 );
                  addtraps--;
                }
                nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer, gameConstants.playerMaxDeaths );

                nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "SetNumberInList", PhotonTargets.AllBufferedViaServer, playercount );
            }



            scoreBoard.transform.GetChild( playercount ).GetChild( 5 ).GetComponent<RawImage>().color = nextPlayerInOrder.GetComponent<SpriteRenderer>().color = nextPlayerInOrder.GetComponent<Player>().colors[nextPlayerInOrder.GetComponent<Player>().photonView.ownerId];

            nextPlayerInOrder.transform.position = rooms.GetChild(playercount).position;
            if(nextPlayerInOrder.GetComponent<Player>().cam != null){
              //set the camera to the player spawn location while keeping it's Z coordinate
                Transform playercam = nextPlayerInOrder.GetComponent<Player>().cam.transform;
              playercam.position = new Vector3( nextPlayerInOrder.transform.position.x, nextPlayerInOrder.transform.position.y,playercam.position.z) ;
            }

              playercount++;
              //if there is somehow more players than there are scoreboard elements break out of the loop
            if ( playercount >= scoreBoard.transform.childCount ) { return; }

        }
        //disable all unused scorecards
        while ( playercount < scoreBoard.transform.childCount )
        {
            scoreBoard.transform.GetChild( playercount ).gameObject.active = false;
            playercount++;
        }







    }



    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

        if ( stream.isWriting )
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

    [PunRPC]
    public void rpcPlayerSetTrapForHidingSpot(int player, int whichHidingSpot, int whattrap)
    {
      if ( PhotonNetwork.isMasterClient )
      {
        foreach(Transform el in playerManager.transform)
        {
            //check that the player has the trap to use
            if(el.GetComponent<PhotonView>().ownerId == player )
            {
              //check that the hiding spot exists and isnt already trapped
              HidingSpot temphidingspot = hidingSpotManager.GetHidingSpot(whichHidingSpot);
              if(temphidingspot != null )
              {
                if( temphidingspot.trapValue != 0)
                {
                  el.GetComponent<Player>().ServerUpdateLives(1);
                  //set the spot to no longer be trapped since it was just used
                  this.photonView.RPC( "rpcNewScrollLine", PhotonTargets.AllViaServer, "Trap went off");
                  this.photonView.RPC( "rpcSetTrapForHidingSpot", PhotonTargets.AllBufferedViaServer, whichHidingSpot,0 );
                }
                  else
                {
                      if(el.GetComponent<Player>().GetInventory().SelectTrap(whattrap))
                      {
                        el.GetComponent<PhotonView>().RPC( "rpcSetEquippedTrap", PhotonTargets.AllBufferedViaServer, 0);
                        el.GetComponent<PhotonView>().RPC( "AddorRemoveTrap", PhotonTargets.AllBufferedViaServer, whattrap ,0);
                        this.photonView.RPC( "rpcSetTrapForHidingSpot", PhotonTargets.AllBufferedViaServer, whichHidingSpot,whattrap );
                      }
                }
              }
              return;
            }
        }

      }
    }

    [PunRPC]
    public void rpcSetTrapForHidingSpot(int whichHidingSpot, int whattrap)
    {

      hidingSpotManager.SetTrapForHidingSpot(whichHidingSpot,whattrap);
    }

    [PunRPC]
    public void rpcSetCollectibleForHidingSpot(int whichHidingSpot, int whatitem)
    {
      hidingSpotManager.SetCollectibleForHidingSpot(whichHidingSpot,whatitem);
    }

    [PunRPC]
    public void rpcNewScrollLine(string newline)
    {
      scrollingText.NewLine(newline);
    }

    [PunRPC]
    public void OpenDoor( int whichDoor,bool open)
    {
        hidingSpotManager.OpenDoor(whichDoor,open);
    }

    [PunRPC]
    public void OpenHidingSpot(int whichPlayer, int whichHidingSpot)
    {

        if ( PhotonNetwork.isMasterClient )
        {
              Player actingPlayer = null;
              foreach(Transform go in playerManager.transform)
              {
                  if(go.GetComponent<PhotonView>().ownerId == whichPlayer)
                  {
                    actingPlayer = go.GetComponent<Player>();


                  }
              }

              //check that the hiding spot is in the list range
              if(hidingSpotManager.hidingSpots.hasEntity(whichHidingSpot))
              {

                  HidingSpot activatedHidingSpot = hidingSpotManager.hidingSpots[whichHidingSpot];
                  //if trapped, activate, otherwise check for hidden object
                  // TODO: question - placing a trap requires the playing to be holding it out?
                    if(activatedHidingSpot.GetTrap() != 0)
                    {

                      //TODO: trap logic
                      actingPlayer.ServerUpdateLives(1);
                      //set the spot to no longer be trapped since it was just used
                      this.photonView.RPC( "rpcNewScrollLine", PhotonTargets.AllViaServer, "Trap went off");
                      this.photonView.RPC( "rpcSetTrapForHidingSpot", PhotonTargets.AllBufferedViaServer, whichHidingSpot,0 );
                    }
                    else
                    {

                          if(activatedHidingSpot.GetCollectible() != 0)
                          {


                                    if(actingPlayer.GetInventory().CanHoldMoreCollectibles() == true )
                                    {

                                      //TODO: add item to player inventory

                                      this.photonView.RPC( "rpcNewScrollLine", PhotonTargets.AllViaServer, "Item Found");
                                      //move the item in the hiding spot to the players inventory
                                      this.photonView.RPC( "rpcSetCollectibleForHidingSpot", PhotonTargets.AllBufferedViaServer, whichHidingSpot,0 );
                                    }

                        }
                    }
                }//end of size check - if block


          }//end of if server


    }





    public void PopulateScoreBoard(int newPlayer, string newname)
    {
        int cardcount = 0;
        int playercount = 0;

        Transform nextPlayerInOrder = playerManager.transform.GetChild( 0 );
        if ( nextPlayerInOrder.GetComponent<Player>().playerNum <= 0 )
        {
            // nextPlayerInOrder.GetComponent<Player>().lives = startingLives;

            nextPlayerInOrder.GetComponent<Player>().playerNum = nextPlayerInOrder.GetComponent<PhotonView>().ownerId;
        }
        int lowestPlayerNum = 99999;
        int lastPlayerNum = -1;

        while ( playercount < playerManager.transform.childCount )
        {


            foreach ( Transform go in playerManager.transform )
            {

                //So the new players has their name when they join
                if ( go.GetComponent<Player>().name.Length <= 0 )
                {
                    go.GetComponent<Player>().name = newname;

                }
                if ( go.GetComponent<Player>().playerNum <= 0 )
                {
                    go.GetComponent<Player>().playerNum = go.GetComponent<PhotonView>().ownerId;

                }
                if ( go.GetComponent<Player>().playerNum > lastPlayerNum && go.GetComponent<Player>().playerNum < lowestPlayerNum )
                {
                    nextPlayerInOrder = go;
                    lowestPlayerNum = go.GetComponent<Player>().playerNum;
                }

            }
            lastPlayerNum = lowestPlayerNum;
            lowestPlayerNum = 99999;

            scoreBoard.transform.GetChild( playercount ).gameObject.active = true;

            nextPlayerInOrder.GetComponent<Player>().myScoreCard = scoreBoard.transform.GetChild( playercount ).gameObject;
            // nextPlayerInOrder.parent = playerManager.transform;
            if ( PhotonNetwork.isMasterClient )
            {
                nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer, nextPlayerInOrder.GetComponent<Player>().lives );
                // nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "UpdateScore", PhotonTargets.AllBufferedViaServer, nextPlayerInOrder.GetComponent<Player>().score );
                // nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "UpdatePower", PhotonTargets.AllBufferedViaServer, nextPlayerInOrder.GetComponent<Player>().money );
                nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "SetNumberInList", PhotonTargets.AllBufferedViaServer, playercount );
            }


            scoreBoard.transform.GetChild( playercount ).GetChild( 5 ).GetComponent<RawImage>().color = nextPlayerInOrder.GetComponent<SpriteRenderer>().color = nextPlayerInOrder.GetComponent<Player>().colors[nextPlayerInOrder.GetComponent<Player>().photonView.ownerId];

            // scoreBoard.transform.GetChild(playercount).GetChild(5).GetComponent<RawImage>().color = colors[playercount].color;
            playercount++;
            if ( playercount >= scoreBoard.transform.childCount ) { return; }
        }
        while ( playercount < scoreBoard.transform.childCount )
        {
            scoreBoard.transform.GetChild( playercount ).gameObject.active = false;
            playercount++;
        }

    }


    public void OnMasterClientSwitched(PhotonPlayer player)
    {
        Debug.Log( "XXXXXXOnMasterClientSwitched: " + player );


    }

    public void OnLeftRoom()
    {
        Debug.Log( "XXXXXXOnLeftRoom (local)" );

        // back to main menu
        // SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log( "XXXXOnDisconnectedFromPhoton" );

        // back to main menu
        // SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log( "XXXXXOnPhotonInstantiate " + info.sender );    // you could use this info to store this or react
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {

        // Debug.Log("XXXXXX  OnPhotonPlayerConnected: " + player);

        // PhotonNetwork.playerList[player.ID - 1].SetCustomProperties(hash);

        // GameObject clone = PhotonNetwork.Instantiate(this.playerPrefab.name, transform.position, Quaternion.identity, 0) ;

    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log( "OnPlayerDisconneced: " + player );
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log( "OnFailedToConnectToPhoton" );

        // back to main menu
        // SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
    }
}
