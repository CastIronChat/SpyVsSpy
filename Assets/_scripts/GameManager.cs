using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Photon.MonoBehaviour
{
    // HACK anyone can use this method to get a reference to the GameManager assuming it is a global singleton
    public static GameManager instance {
        get => singleton.instance;
    }
    private static GlobalSingletonGetter<GameManager> singleton = new GlobalSingletonGetter<GameManager>(gameObjectName: "GameManager");
    public PlayerManager playerManager;
    public HidingspotManager hidingSpotManager; //Track all the hiding spots in a single place rather than have each hiding spot handle itself
    public ScrollingText scrollingText;
    public RoundManager roundManager;

    public GameConstants gameConstants
    {
        get => GameConstants.instance;
    }

    public GameObject playerPrefab;
    public GameObject scoreBoard,startbutton;
    public int activePlayers;
    public Renderer myRenderer;
    public List<Material> colors;
    public IconRowHUD playerinventoryimages;
    public IconRowHUD playertrapimages;
    public Transform rooms;
    public Transform idleplayerManager;

    //placeholder that needs to be moved to the master trap logic
    public GameObject debugExplosion;

    public void Awake()
    {
        // Tell photon how to send a `TrapType` over the network, by transmitting the TrapType's Id and looking up the TrapType instance
        // on the receiving end.
        // Can enable the others if/when we start using a Registry to track them
        // PhotonPeer.RegisterType(typeof(Door), (byte)1, hidingSpotManager.doors.SerializeEntityReference, hidingSpotManager.doors.DeserializeEntityReference);
        PhotonPeer.RegisterType(typeof(TrapType), (byte)2, RegistryHelper.SerializeEntityReference, RegistryHelper.DeserializeEntityReference);
        // PhotonPeer.RegisterType(typeof(Door), (byte)1, hidingSpotManager.doors.SerializeEntityReference, hidingSpotManager.doors.DeserializeEntityReference);
        if ( !PhotonNetwork.connected )
        {

        }
        if ( PhotonNetwork.isMasterClient )
        {
                startbutton.SetActive( true );
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
        if ( playerManager.activePlayers.Count > 1 )
        {
            StartRound();
        }

    }

    [PunRPC]
    public void StartRound()
    {

        startbutton.SetActive( false );

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

            scoreBoard.transform.GetChild( playercount ).gameObject.SetActive( true );
            scoreBoard.transform.GetChild( playercount ).GetChild( 1 ).GetComponent<Text>().text = nextPlayerInOrder.GetComponent<Player>().name + " : ";
            scoreBoard.transform.GetChild( playercount ).GetChild( 2 ).GetComponent<Text>().text = nextPlayerInOrder.GetComponent<Player>().score.ToString();

            nextPlayerInOrder.GetComponent<Player>().myScoreCard = scoreBoard.transform.GetChild( playercount ).gameObject;
            nextPlayerInOrder.parent = playerManager.transform;
            if ( PhotonNetwork.isMasterClient )
            {
                int addtraps = gameConstants.startingTraps;
                while(addtraps > 0)
                {
                  nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "AddorRemoveTrap", PhotonTargets.AllBufferedViaServer, gameConstants.trapTypes[addtraps],1 );
                  addtraps--;
                }
                nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer, gameConstants.playerMaxDeaths );

                nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "SetNumberInList", PhotonTargets.AllBufferedViaServer, playercount );
            }



            scoreBoard.transform.GetChild( playercount ).GetChild( 5 ).GetComponent<RawImage>().color = nextPlayerInOrder.GetComponent<SpriteRenderer>().color = nextPlayerInOrder.GetComponent<Player>().colors[nextPlayerInOrder.GetComponent<Player>().photonView.ownerId];

            nextPlayerInOrder.transform.position = rooms.GetChild(playercount).position;

            playercount++;
            //if there is somehow more players than there are scoreboard elements break out of the loop
            if ( playercount >= scoreBoard.transform.childCount ) { return; }

        }
        //disable all unused scorecards
        while ( playercount < scoreBoard.transform.childCount )
        {
            scoreBoard.transform.GetChild( playercount ).gameObject.SetActive( false );
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


    public void PlayerEnterHidingSpot(int playerid, int trapspot, TrapType trapvalue)
    {
          if ( PhotonNetwork.isMasterClient )
          {
                this.photonView.RPC( "ActivateTrapEffect", PhotonTargets.AllViaServer, playerid, trapspot, trapvalue);
          }
    }

    public void PlayerBumpHidingSpot(int playerid, int trapspot, TrapType trapvalue)
    {
          if ( PhotonNetwork.isMasterClient )
          {
                hidingSpotManager.GetHidingSpot(trapspot).PlayAnimation("bump");
                this.photonView.RPC( "ActivateTrapEffect", PhotonTargets.AllViaServer, playerid, trapspot, trapvalue);
          }
    }

    public void SyncPyhsicsLocation(int hidingSpot)
    {
      if ( PhotonNetwork.isMasterClient )
      {
          this.photonView.RPC( "rpcSyncPyhsicsLocation", PhotonTargets.AllViaServer,hidingSpot, hidingSpotManager.GetHidingSpot(hidingSpot).transform.position);
      }
    }

    [PunRPC]
    public void rpcSyncPyhsicsLocation(int hidingSpot, Vector3 realPos)
    {
      hidingSpotManager.GetHidingSpot(hidingSpot).transform.position = realPos;
    }

    [PunRPC]
    public void CreateExplosion(Vector3 explosionLocation)
    {
      //The trap effects should be purely visual so spawning a local prefab for each player rather than a network object makes this simplier. The explosion should have a die in time script to clean itself up
      Instantiate(debugExplosion,explosionLocation,debugExplosion.transform.rotation);
    }


    [PunRPC]
    public void ActivateTrapEffect(int actingPlayer,int whichHidingSpot, TrapType traptype)
    {
          TrapType tempTrap = traptype;
          Player acting_Player = null;
          Transform closestRoom = rooms.GetChild(0);

          //find the room the player is in


          foreach(Player player in playerManager.activePlayers)
          {
              //check that the player has the trap to use
              if(player.GetComponent<PhotonView>().ownerId == actingPlayer )
              {
                acting_Player = player;
              }
          }
          // foreach(Transform room in rooms)
          // {
          //   if( Vector3.Distance(acting_Player.transform.position, room.position) < Vector3.Distance(acting_Player.transform.position, closestRoom.position))
          //   {closestRoom = room;}
          // }

          //The trap effects should be purely visual so spawning a local prefab for each player rather than a network object makes this simplier. The explosion should have a die in time script to clean itself up
          if(tempTrap.spawnOnPlayer == true)
          {
            Instantiate(tempTrap.trapEffect,acting_Player.transform.position,debugExplosion.transform.rotation);
          }
          else
          {
            Instantiate(tempTrap.trapEffect, hidingSpotManager.GetHidingSpot(whichHidingSpot).transform.position,debugExplosion.transform.rotation);
          }

          if ( PhotonNetwork.isMasterClient )
          {
                if(tempTrap.hasKnockback == true)
                {
                    Vector3 dir = (acting_Player.transform.position - hidingSpotManager.GetHidingSpot(whichHidingSpot).transform.position).normalized;
                    acting_Player.GetComponent<PhotonView>().RPC( "rpcGetThrownByTrap", PhotonTargets.AllViaServer, dir * tempTrap.knockbackForce, 1.0f);
                }

                acting_Player.ServerUpdateLives(tempTrap.oneTimeDamage);
                //set the spot to no longer be trapped since it was just used
                this.photonView.RPC( "rpcNewScrollLine", PhotonTargets.AllViaServer, traptype.name);
                this.photonView.RPC( "rpcSetTrapForHidingSpot", PhotonTargets.AllBufferedViaServer, whichHidingSpot, null );


          }
    }


    [PunRPC]
    public void AnimateHidingSpot(int whichHidingSpot,string animation)
    {
      //The trap effects should be purely visual so spawning a local prefab for each player rather than a network object makes this simplier. The explosion should have a die in time script to clean itself up
       hidingSpotManager.GetHidingSpot(whichHidingSpot).PlayAnimation(animation);
    }

    [PunRPC]
    public void rpcPlayerSetTrapForHidingSpot(int playerId, int whichHidingSpot, TrapType trapType)
    {

      // this.photonView.RPC( "AnimateHidingSpot", PhotonTargets.AllViaServer, whichHidingSpot);
      if ( PhotonNetwork.isMasterClient )
      {
        foreach(Player player in playerManager.activePlayers)
        {
            //check that the player has the trap to use
            if(player.GetComponent<PhotonView>().ownerId == playerId )
            {
              //check that the hiding spot exists and isnt already trapped
                  HidingSpot temphidingspot = hidingSpotManager.GetHidingSpot(whichHidingSpot);
                  if(temphidingspot != null )
                  {
                        if( temphidingspot.trapValue != null)
                        {
                          this.photonView.RPC( "ActivateTrapEffect", PhotonTargets.AllViaServer, playerId, whichHidingSpot, temphidingspot.trapValue);


                        }
                          else
                        {
                          //always set the sprite to blank, even if the player doesnt have a trap, because it that scenario its a server/client/client info mismatch
                          player.photonView.RPC( "rpcSetEquippedTrap", PhotonTargets.AllBufferedViaServer, gameConstants.trapTypes[0]);
                              if(player.GetInventory().HasTrap(trapType))
                              {

                                player.photonView.RPC( "AddorRemoveTrap", PhotonTargets.AllBufferedViaServer, trapType,0);
                                this.photonView.RPC( "rpcSetTrapForHidingSpot", PhotonTargets.AllBufferedViaServer, whichHidingSpot,trapType);
                              }

                        }
                  }
              return;
            }
        }

      }
    }

    [PunRPC]
    public void rpcSetTrapForHidingSpot(int whichHidingSpot, TrapType trapType)
    {
        hidingSpotManager.SetTrapForHidingSpot(whichHidingSpot, trapType);
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
    public void OpenDoor( int whichDoor, bool open )
    {
      hidingSpotManager.OpenDoor(whichDoor, open);
    }

    [PunRPC]
    public void OpenHidingSpot(int whichPlayer, int whichHidingSpot)
    {
      hidingSpotManager.GetHidingSpot(whichHidingSpot).PlayAnimation("search");
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
              if(whichHidingSpot < hidingSpotManager.hidingSpots.Count)
              {

                  HidingSpot activatedHidingSpot = hidingSpotManager.hidingSpots[whichHidingSpot];
                  //if trapped, activate, otherwise check for hidden object
                  // TODO: question - placing a trap requires the playing to be holding it out?
                    if(activatedHidingSpot.GetTrap() != null)
                    {

                      this.photonView.RPC( "ActivateTrapEffect", PhotonTargets.AllViaServer, whichPlayer, whichHidingSpot, activatedHidingSpot.trapValue);

                    }
                    else
                    {

                          if(activatedHidingSpot.GetCollectible() != 0)
                          {
                            //-1 is the briefcase | use negative numbers for special conditions
                            if(activatedHidingSpot.GetCollectible() == -1)
                            {
                              this.photonView.RPC( "rpcNewScrollLine", PhotonTargets.AllViaServer, "Briefcase Found");
                              //set the hiding spot to no longer have a collectible
                              this.photonView.RPC( "rpcSetCollectibleForHidingSpot", PhotonTargets.AllBufferedViaServer, whichHidingSpot,0 );
                            }
                              else
                              {
                                if(actingPlayer.GetInventory().CanHoldMoreCollectibles() == true )
                                {
                                  actingPlayer.GetComponent<PhotonView>().RPC( "AddCollectible", PhotonTargets.AllBufferedViaServer, activatedHidingSpot.collectibleValue, 1);


                                  this.photonView.RPC( "rpcNewScrollLine", PhotonTargets.AllViaServer, "Item Found");


                                  //move the item in the hiding spot to the players inventory
                                  this.photonView.RPC( "rpcSetCollectibleForHidingSpot", PhotonTargets.AllBufferedViaServer, whichHidingSpot,0 );
                                }
                              }



                        }
                    }
                }//end of size check - if block


          }//end of if server


    }





    public void PopulateScoreBoard(int newPlayer, string newname)
    {
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

            scoreBoard.transform.GetChild( playercount ).gameObject.SetActive( true );

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
            scoreBoard.transform.GetChild( playercount ).gameObject.SetActive( false );
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
