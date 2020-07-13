using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CastIronChat.EntityRegistry;
using ComponentReferenceAttributes;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class GameManager : Photon.MonoBehaviour
{
    // HACK anyone can use this method to get a reference to the GameManager assuming it is a global singleton
    public static GameManager instance
    {
        get => singleton.instance;
    }

    private static GlobalSingletonGetter<GameManager> singleton =
        new GlobalSingletonGetter<GameManager>( gameObjectName: "GameManager" );

    [ChildComponent]
    public PlayerManager playerManager;

    [ChildComponent] public HidingspotManager
        hidingSpotManager; //Track all the hiding spots in a single place rather than have each hiding spot handle itself

    [ChildComponent] public CollectibleManager collectibleManager;
    [SiblingComponent] public Map map;
    public ScrollingText scrollingText;
    public RoundManager roundManager;

    public GameConstants gameConstants
    {
        get => GameConstants.instance;
    }

    public GameObject playerPrefab;
    public GameObject scoreBoard, startbutton, bubbleHidingspot, bubblePlayer;
    public int activePlayers;
    public Renderer myRenderer;
    public List<Material> colors;
    public IconRowHUD playerinventoryimages;
    public IconRowHUD playertrapimages;
    public Transform rooms {
        get => map.rooms;
    }
    public Transform idleplayerManager;

    //placeholder that needs to be moved to the master trap logic
    public GameObject debugExplosion;

    public void Awake()
    {
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


    /// <summary>
    /// Called on all clients by a newly-spawned Player instance to announce itself.
    /// </summary>
    /// <param name="newPlayerId">ID of the new player so we can look it up locally.</param>
    [PunRPC]
    public void PlayerJoinGame(int newPlayerId, string newname)
    {
        print( "new player Number: " + newPlayerId );
        var player = playerManager.activePlayers.First( p => p.playerId == newPlayerId );
        Assert.IsNotNull(player);
        player.name = newname;
        reSortPlayers();
        initNewPlayerForGameplay( player );
        reassignScoreboards();
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

        // Move all idle players onto playerManager
        foreach ( Transform go in idleplayerManager )
        {
            go.transform.parent = playerManager.transform;
        }

        reSortPlayers();

        var allPlayers = new List<Player>( playerManager.GetComponentsInChildren<Player>() );

        for ( var playerIndex = 0;
            playerIndex < allPlayers.Count && playerIndex < scoreBoard.transform.childCount;
            playerIndex++ )
        {
            var player = allPlayers[playerIndex];
            initNewPlayerForGameplay( player );

            // place the player into the room matching this player's index
            player.transform.position = rooms.GetChild( playerIndex ).position;
        }
    }

    /// <summary>
    /// Initialize a new Player for the start of gameplay, either at round start or as soon as they join the server
    /// if they joined late.
    /// </summary>
    private void initNewPlayerForGameplay(Player player)
    {
        if ( PhotonNetwork.isMasterClient )
        {
            int addtraps = gameConstants.startingTraps;
            while ( addtraps > 0 )
            {
                player.photonView.RPC( "AddorRemoveTrap", PhotonTargets.AllBufferedViaServer,
                    gameConstants.trapTypes[addtraps], 1 );
                addtraps--;
            }

            player.photonView.RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer,
                gameConstants.playerMaxDeaths );
        }
    }

    /// <summary>
    /// Put players in canonical ordering, sorted by playerId
    /// </summary>
    private void reSortPlayers()
    {
        var allPlayers = new List<Player>( playerManager.GetComponentsInChildren<Player>() );
        allPlayers.Sort(
            (playerA, playerB) => playerA.playerId > playerB.playerId ? 1 : -1
        );
        for ( var playerIndex = 0;
            playerIndex < allPlayers.Count && playerIndex < scoreBoard.transform.childCount;
            playerIndex++ )
        {
            var player = allPlayers[playerIndex];
            player.transform.SetSiblingIndex( playerIndex );
        }
    }

    /// <summary>
    /// Re-assign scoreboards to players.  Should be re-done when someone joins or leaves the game.
    /// </summary>
    private void reassignScoreboards()
    {
        var allPlayers = new List<Player>( playerManager.GetComponentsInChildren<Player>() );

        for ( var playerIndex = 0;
            playerIndex < allPlayers.Count && playerIndex < scoreBoard.transform.childCount;
            playerIndex++ )
        {
            var player = allPlayers[playerIndex];
            var scoreboardForPlayer = scoreBoard.transform.GetChild( playerIndex );
            scoreboardForPlayer.gameObject.SetActive( true );
            scoreboardForPlayer.GetChild( 1 ).GetComponent<Text>().text = player.name + " : ";
            scoreboardForPlayer.GetChild( 2 ).GetComponent<Text>().text = player.score.ToString();

            player.myScoreCard = scoreboardForPlayer.gameObject;

            scoreboardForPlayer.GetChild( 5 ).GetComponent<RawImage>().color =
                player.GetComponent<SpriteRenderer>().color =
                    player.colors[player.playerIndex];
        }

        //disable all unused scorecards
        for ( var i = allPlayers.Count; i < scoreBoard.transform.childCount; i++ )
        {
            scoreBoard.transform.GetChild( i ).gameObject.SetActive( false );
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
            this.photonView.RPC( "ActivateTrapEffect", PhotonTargets.AllViaServer, playerid, trapspot, trapvalue );
        }
    }

    public void PlayerBumpHidingSpot(int playerid, int trapspot, TrapType trapvalue)
    {
        if ( PhotonNetwork.isMasterClient )
        {
            hidingSpotManager.GetHidingSpot( trapspot ).PlayAnimation( "bump" );
            this.photonView.RPC( "ActivateTrapEffect", PhotonTargets.AllViaServer, playerid, trapspot, trapvalue );
        }
    }

    public void SyncPyhsicsLocation(int hidingSpot)
    {
        if ( PhotonNetwork.isMasterClient )
        {
            this.photonView.RPC( "rpcSyncPyhsicsLocation", PhotonTargets.AllViaServer, hidingSpot,
                hidingSpotManager.GetHidingSpot( hidingSpot ).transform.position );
        }
    }

    [PunRPC]
    public void rpcSyncPyhsicsLocation(int hidingSpot, Vector3 realPos)
    {
        hidingSpotManager.GetHidingSpot( hidingSpot ).transform.position = realPos;
    }

    [PunRPC] //remove, unused
    public void CreateExplosion(Vector3 explosionLocation)
    {
        //The trap effects should be purely visual so spawning a local prefab for each player rather than a network object makes this simplier. The explosion should have a die in time script to clean itself up
        Instantiate( debugExplosion, explosionLocation, debugExplosion.transform.rotation );
    }


    [PunRPC]
    public void ActivateTrapEffect(int actingPlayer, int whichHidingSpot, TrapType traptype)
    {
        TrapType tempTrap = traptype;
        HidingSpot hidingSpot = hidingSpotManager.GetHidingSpot(whichHidingSpot);

        Player acting_Player = playerManager.getPlayerById( actingPlayer );

        //find the room the player is in
        // foreach(Transform room in rooms)
        // {
        //   if( Vector3.Distance(acting_Player.transform.position, room.position) < Vector3.Distance(acting_Player.transform.position, closestRoom.position))
        //   {closestRoom = room;}
        // }

        //The trap effects should be purely visual so spawning a local prefab for each player rather than a network object makes this simplier. The explosion should have a die in time script to clean itself up
        if ( tempTrap.spawnOnPlayer == true )
        {
            Instantiate( tempTrap.trapEffect, acting_Player.transform.position, debugExplosion.transform.rotation );
        }
        else
        {
            Instantiate( tempTrap.trapEffect, hidingSpot.transform.position,
                debugExplosion.transform.rotation );
        }

        if ( PhotonNetwork.isMasterClient )
        {
            if ( tempTrap.hasKnockback == true )
            {
                Vector3 dir = (acting_Player.transform.position -
                               hidingSpot.transform.position).normalized;
                acting_Player.GetComponent<PhotonView>().RPC( "rpcGetThrownByTrap", PhotonTargets.AllViaServer,
                    dir * tempTrap.knockbackForce, 1.0f );
            }

            if(tempTrap.inputLockOut != 0)
            {
                Vector3 dir = (acting_Player.transform.position - hidingSpotManager.GetHidingSpot(whichHidingSpot).transform.position).normalized;
                acting_Player.GetComponent<PhotonView>().RPC( "rpcSetInputLockOut", PhotonTargets.AllViaServer, tempTrap.inputLockOut);
                acting_Player.GetComponent<PhotonView>().RPC( "rpcPlayAnimation", PhotonTargets.AllViaServer, "freeze");

            }

            acting_Player.ServerUpdateLives( tempTrap.oneTimeDamage );
            //set the spot to no longer be trapped since it was just used
            this.photonView.RPC( "rpcNewScrollLine", PhotonTargets.AllViaServer, traptype.name );
            this.photonView.RPC( "rpcSetTrapForHidingSpot", PhotonTargets.AllBufferedViaServer, whichHidingSpot, null );
        }
    }


    [PunRPC]
    public void AnimateHidingSpot(int whichHidingSpot, string animation)
    {
        //The trap effects should be purely visual so spawning a local prefab for each player rather than a network object makes this simplier. The explosion should have a die in time script to clean itself up
        hidingSpotManager.GetHidingSpot( whichHidingSpot ).PlayAnimation( animation );
    }

    [PunRPC]
    public void rpcPlayerSetTrapForHidingSpot(int playerId, int whichHidingSpot, TrapType trapType)
    {
        // this.photonView.RPC( "AnimateHidingSpot", PhotonTargets.AllViaServer, whichHidingSpot);
        if ( PhotonNetwork.isMasterClient )
        {
            foreach ( Player player in playerManager.activePlayers )
            {
                //check that the player has the trap to use
                if ( player.playerId == playerId )
                {
                    //check that the hiding spot exists and isnt already trapped
                    HidingSpot temphidingspot = hidingSpotManager.GetHidingSpot( whichHidingSpot );
                    if ( temphidingspot != null )
                    {
                        if ( temphidingspot.trapValue != null )
                        {
                            this.photonView.RPC( "ActivateTrapEffect", PhotonTargets.AllViaServer, playerId,
                                whichHidingSpot, temphidingspot.trapValue );
                        }
                        else
                        {
                            //always set the sprite to blank, even if the player doesnt have a trap, because it that scenario its a server/client/client info mismatch
                            player.photonView.RPC( "rpcSetEquippedTrap", PhotonTargets.AllBufferedViaServer,
                                gameConstants.trapTypes[0] );
                            if ( player.GetInventory().HasTrap( trapType ) )
                            {
                                player.photonView.RPC( "AddorRemoveTrap", PhotonTargets.AllBufferedViaServer, trapType,
                                    0 );
                                this.photonView.RPC( "rpcSetTrapForHidingSpot", PhotonTargets.AllBufferedViaServer,
                                    whichHidingSpot, trapType );

                                photonView.RPC( "SetBubbles", PhotonTargets.AllViaServer, whichHidingSpot, playerId, 0,
                                    0, trapType.uniqueId, 0 );
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
        hidingSpotManager.SetTrapForHidingSpot( whichHidingSpot, trapType );
    }

    [PunRPC]
    public void rpcSetCollectibleForHidingSpot(int whichHidingSpot, int whatitem)
    {
        hidingSpotManager.SetCollectibleForHidingSpot( whichHidingSpot, whatitem );
    }

    [PunRPC]
    public void rpcNewScrollLine(string newline)
    {
        scrollingText.NewLine( newline );
    }

    [PunRPC]
    public void OpenDoor(int whichDoor, bool open)
    {
        hidingSpotManager.OpenDoor( whichDoor, open );
    }

    public void EnableBubbles()
    {
        bubblePlayer.transform.rotation = transform.rotation;
        bubbleHidingspot.transform.rotation = transform.rotation;
        bubblePlayer.GetComponent<Animator>().Play( "turnoff" );
        bubbleHidingspot.GetComponent<Animator>().Play( "turnoff" );
    }

    [PunRPC]
    public void SetBubbles(int whichspot, int whichplayer, int collectible, int collectibleInSpot, int trapInSpot,
        int weaponInSpot)
    {
        //move the search bubbles that display the items pulled from or put in a hidingspot to the player and spots Location
        //then tell the localplayer to enable them so only the local player sees
        //NOTE: idea -a trap could make it so that everyone can see
        foreach ( Transform go in playerManager.transform )
        {
            if ( go.GetComponent<Player>().playerId == whichplayer )
            {
                bubblePlayer.transform.position = go.position;
                bubblePlayer.transform.GetChild( 0 ).GetComponent<SpriteRenderer>().sprite =
                    gameConstants.collectibleTypes[collectible].sprite;
                bubbleHidingspot.transform.position = hidingSpotManager.GetHidingSpot( whichspot ).transform.position;
                //pass the value to avoid a race condition

                if ( collectibleInSpot != 0 )
                {
                    bubbleHidingspot.transform.GetChild( 0 ).GetComponent<SpriteRenderer>().sprite =
                        gameConstants.collectibleTypes[collectibleInSpot].sprite;
                }
                else if ( trapInSpot != 0 )
                {
                    bubbleHidingspot.transform.GetChild( 0 ).GetComponent<SpriteRenderer>().sprite =
                        gameConstants.trapTypes[trapInSpot].sprite;
                }
                // else if (weaponInSpot != 0)
                // {bubbleHidingspot.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = gameConstants.weaponTypes[weaponInSpot].sprite;}


                go.GetComponent<Player>().BubbleAnimation();
                bubblePlayer.transform.parent = go;
            }
        }
    }


    [PunRPC]
    public void OpenHidingSpot(int whichPlayer, int whichHidingSpot)
    {
        hidingSpotManager.GetHidingSpot( whichHidingSpot ).PlayAnimation( "search" );
        if ( PhotonNetwork.isMasterClient )
        {
            Player actingPlayer = null;
            foreach ( Transform go in playerManager.transform )
            {
                if ( go.GetComponent<Player>().playerId == whichPlayer )
                {
                    actingPlayer = go.GetComponent<Player>();
                }
            }

            //check that the hiding spot is in the list range
            if ( whichHidingSpot < hidingSpotManager.hidingSpots.Count )
            {
                HidingSpot activatedHidingSpot = hidingSpotManager.hidingSpots[whichHidingSpot];
                //if trapped, activate, otherwise check for hidden object
                // TODO: question - placing a trap requires the playing to be holding it out?
                if ( activatedHidingSpot.GetTrap() != null )
                {
                    this.photonView.RPC( "ActivateTrapEffect", PhotonTargets.AllViaServer, whichPlayer, whichHidingSpot,
                        activatedHidingSpot.trapValue );
                }
                else
                {
                    if ( activatedHidingSpot.GetCollectible() != 0 )
                    {
                        //-1 is the briefcase | use negative numbers for special conditions
                        if ( activatedHidingSpot.GetCollectible() == -1 )
                        {
                            this.photonView.RPC( "rpcNewScrollLine", PhotonTargets.AllViaServer, "Briefcase Found" );
                            //set the hiding spot to no longer have a collectible
                            this.photonView.RPC( "rpcSetCollectibleForHidingSpot", PhotonTargets.AllBufferedViaServer,
                                whichHidingSpot, 0 );
                        }
                        else
                        {
                            if ( actingPlayer.GetInventory().CanHoldMoreCollectibles() == true )
                            {
                                int collectiblegrabbed = activatedHidingSpot.GetCollectible();
                                actingPlayer.GetComponent<PhotonView>().RPC( "AddCollectible",
                                    PhotonTargets.AllBufferedViaServer, activatedHidingSpot.collectibleValue, 1 );


                                this.photonView.RPC( "rpcNewScrollLine", PhotonTargets.AllViaServer, "Item Found" );


                                //move the item in the hiding spot to the players inventory
                                this.photonView.RPC( "rpcSetCollectibleForHidingSpot",
                                    PhotonTargets.AllBufferedViaServer, whichHidingSpot, 0 );

                                //set the bubbles to show the item picked up
                                photonView.RPC( "SetBubbles", PhotonTargets.AllViaServer, whichHidingSpot, whichPlayer,
                                    collectiblegrabbed, 0, 0, 0 );
                            }
                        }
                    }
                }
            } //end of size check - if block
        } //end of if server
    }

[PunRPC]
public void DisarmHidingSpot(int whichHidingSpot)
{

if ( PhotonNetwork.isMasterClient )
{
    //check that the hiding spot is in the list range
    if ( whichHidingSpot < hidingSpotManager.hidingSpots.Count )
    {

        HidingSpot activatedHidingSpot = hidingSpotManager.hidingSpots[whichHidingSpot];
        this.photonView.RPC( "rpcSetTrapForHidingSpot", PhotonTargets.AllBufferedViaServer, whichHidingSpot,
            null );
    }

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
        Debug.Log( "XXXXXOnPhotonInstantiate " + info.sender ); // you could use this info to store this or react
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
