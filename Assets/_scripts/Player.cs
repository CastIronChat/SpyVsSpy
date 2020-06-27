using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : Photon.MonoBehaviour
{
    public GameManager gameManager;
    public int numberInList, playerNum, score, lostScore, money;
    public int lives, gamesPlayed, wins;
    public float speed,networkspeed = 12.0f,interactDistance = 0.4f;
    public string name;
    public GameObject myScoreCard,cam;
    public Material myColor;
    public List<Color> colors;
    /// TODO support diagonal movement?
    private CardinalDirection movementDirection = CardinalDirection.None,facingDirection = CardinalDirection.None;
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerInput input = new PlayerInput();
    private Inventory inventory;
    private Vector3 serverPos;
    private Quaternion serverRot;
    private SpriteRenderer heldSprite;

    void Start()
    {
        if ( this.photonView.ownerId < colors.Count )
        { GetComponent<SpriteRenderer>().color = colors[this.photonView.ownerId]; }

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find( "GameManager" ).GetComponent<GameManager>();
        transform.parent = gameManager.playerManager.transform;
        heldSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        if ( photonView.isMine )
        {

            rb.isKinematic = false;
        }

    }
    void OnEnable()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        inventory = new Inventory();
        inventory.traps = new List<int>();
        inventory.collectibles = new List<int>();
        inventory.traps.Add(0);
        inventory.traps.Add(1);
        inventory.traps.Add(1);
        inventory.traps.Add(1);
        inventory.collectibles.Add(0);
        heldSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();

        gameManager = GameObject.Find( "GameManager" ).GetComponent<GameManager>();
        transform.parent = gameManager.playerManager.transform;
        if ( photonView.isMine )
        {
          cam = GameObject.Find("Main Camera");
            playerNum = this.photonView.ownerId;
            name = PhotonNetwork.playerName;
            gameManager.photonView.RPC( "PlayerJoinGame", PhotonTargets.AllBufferedViaServer, playerNum, name );
            this.photonView.RPC( "JoinGame", PhotonTargets.AllBufferedViaServer, name, photonView.ownerId );

        }
    }


    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if ( stream.isWriting )
        {
            //We own this player: send the others our data
            // stream.SendNext(score);
            // stream.SendNext(name);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //Network player, receive data
            // score = (int)stream.ReceiveNext();
            // name = (string)stream.ReceiveNext();
            serverPos = (Vector3)stream.ReceiveNext();
            serverRot = (Quaternion)stream.ReceiveNext();



        }
    }


    [PunRPC]
    public void JoinGame(string newname, int photonNumber)
    {
        name = newname;
        playerNum = photonNumber;
        numberInList = -1;
        gameManager = GameObject.Find( "GameManager" ).GetComponent<GameManager>();
        transform.parent = gameManager.playerManager.transform;
    }

    [PunRPC]
    public void SetNumberInList(int listPlace)
    {
        numberInList = listPlace;
    }

    [PunRPC]
    public void SetBriefcase(bool setHasBriefcase)
    {
      GetInventory().hasBriefcase = setHasBriefcase;
    }

    public void ServerUpdateScore(int scoreChange)
    {
        score = scoreChange;
        this.photonView.RPC( "UpdateScore", PhotonTargets.AllBufferedViaServer, score );
    }

    [PunRPC]
    public void UpdateScore(int scoreChange)
    {
        score = scoreChange;
        if ( myScoreCard != null )
        {
            myScoreCard.active = true;

        }
    }

    public void UpdateTraps( )
    {
        if ( myScoreCard != null )
        {
            myScoreCard.active = true;
            myScoreCard.transform.GetChild( 1 ).GetComponent<Text>().text = name + " : ";
            myScoreCard.transform.GetChild( 2 ).GetComponent<Text>().text = inventory.GetTrapsString();
        }

        if ( photonView.isMine )
        {
          //update the local players inventory visuals for what traps they have
          inventory.UpdateInventorySprites(gameManager.gameConstants,gameManager.playertrapimages,true);
          inventory.UpdateInventorySprites(gameManager.gameConstants,gameManager.playerinventoryimages,false);
        }

    }

    [PunRPC]
    public void AddorRemoveTrap(int trapUsed,int setto)
    {
        inventory.AddTrap(trapUsed,setto);
        UpdateTraps();
    }

    [PunRPC]
    public void RemoveCollectible(int whichCollectible,int setto)
    {
        inventory.RemoveCollectible(whichCollectible);
        UpdateTraps();
    }

    [PunRPC]
    public void AddCollectible(int whichCollectible,int setto)
    {
        inventory.AddCollectible(whichCollectible,setto);
        UpdateTraps();
    }

    public void ServerUpdateLives(int livesChange)
    {
        lives -= livesChange;
        this.photonView.RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer, lives );
    }

    [PunRPC]
    public void UpdateLives(int livesChange)
    {
        lives = livesChange;
        if ( myScoreCard != null )
        {
            int count = 0;
            string lifeString = "";
            while ( count < lives )
            {
                lifeString = lifeString + "X";
                count++;
            }
            myScoreCard.transform.GetChild( 1 ).GetComponent<Text>().text = name + " : ";
            myScoreCard.transform.GetChild( 3 ).GetComponent<Text>().text = lifeString;
        }
    }




    void Update()
    {
        if ( photonView.isMine )
        {
          if(Input.GetKeyDown(KeyCode.Alpha5))
          {
            inventory.traps = new List<int>();
            inventory.traps.Add(1);
            inventory.traps.Add(1);
            inventory.traps.Add(1);
          }
            Move();
            UseTraps();
            if ( Input.GetKeyDown( KeyCode.Space )  ){TryToInteract();}
        }
        else
        {
          transform.rotation = serverRot;
          if(Vector3.Distance(serverPos,transform.position) > 0.01f)
          {
            transform.position = Vector3.Lerp(transform.position, serverPos, Time.deltaTime * networkspeed);
          }

        }
    }



    [PunRPC]
    public void SetSpriteFlip(bool flip, float rot)
    {
        GetComponent<SpriteRenderer>().flipX = flip;
        transform.eulerAngles = new Vector3( 0, 0, rot );
    }
    [PunRPC]
    public void SetVelocity(Vector3 newvel)
    {
        // rb.velocity = newvel * speed * Time.deltaTime;
    }

    [PunRPC]
    public void SetHeldSprite(int newsprite)
    {
      heldSprite.sprite = gameManager.gameConstants.trapSprites[newsprite];
    }


    public void UseTraps()
    {
      if(Input.GetKeyDown(KeyCode.Alpha1) )
      {

        inventory.equippedTrap = 0;
        gameManager.scrollingText.NewLine("equiped trap 0");
        this.photonView.RPC( "SetHeldSprite", PhotonTargets.AllBufferedViaServer, 0 );

      }
        if(Input.GetKeyDown(KeyCode.Alpha2) && inventory.SelectTrap(1))
        {
          inventory.equippedTrap = 1; gameManager.scrollingText.NewLine("equiped trap 1");
            this.photonView.RPC( "SetHeldSprite", PhotonTargets.AllBufferedViaServer, 1 );
       }
       if(Input.GetKeyDown(KeyCode.Alpha3) && inventory.SelectTrap(2))
       {
         inventory.equippedTrap = 2; gameManager.scrollingText.NewLine("equiped trap 2");
           this.photonView.RPC( "SetHeldSprite", PhotonTargets.AllBufferedViaServer, 2 );
      }
      if(Input.GetKeyDown(KeyCode.Alpha4) && inventory.SelectTrap(3))
      {
        inventory.equippedTrap = 3; gameManager.scrollingText.NewLine("equiped trap 3");
          this.photonView.RPC( "SetHeldSprite", PhotonTargets.AllBufferedViaServer, 3 );
     }
    }


    public void Move()
    {
        // If a movement key is pressed that was not previously, it takes precedence and resets movement to be in that direction.
        // Otherwise movement remains in the same direction as it was before.
        var leftHeld = input.GetDirectionPressed(CardinalDirection.Left);
        var rightHeld = input.GetDirectionPressed(CardinalDirection.Right);
        var upHeld = input.GetDirectionPressed(CardinalDirection.Up);
        var downHeld = input.GetDirectionPressed(CardinalDirection.Down);

        // If no keys held, default to None
        var newDirection = CardinalDirection.None;

        if(leftHeld && !rightHeld) newDirection = CardinalDirection.Left;
        if(!leftHeld && rightHeld) newDirection = CardinalDirection.Right;
        if(upHeld && !downHeld) newDirection = CardinalDirection.Up;
        if(!upHeld && downHeld) newDirection = CardinalDirection.Down;

        // keep same movement direction if possible
        if(movementDirection != CardinalDirection.None && input.GetDirectionPressed(movementDirection)) {
            newDirection = movementDirection;
        }

        // Newly-pressed keys override the movement direction
        foreach (CardinalDirection direction in Enum.GetValues(typeof(CardinalDirection))) {
            if(direction == CardinalDirection.None) continue;
            if(input.GetDirectionDown(direction)) {
                newDirection = direction;
            }
        }
        movementDirection = newDirection;


        if(movementDirection != CardinalDirection.None)
        {facingDirection = movementDirection;}
        // Update velocity
        var movementUnitVector = CardinalDirectionHelper.ToVector3(movementDirection);
        rb.velocity = movementUnitVector * speed * Time.deltaTime;
        this.photonView.RPC( "SetVelocity", PhotonTargets.AllViaServer, movementUnitVector );
        // Update sprite
        switch(movementDirection) {
            case CardinalDirection.Up:
                this.photonView.RPC( "SetSpriteFlip", PhotonTargets.AllViaServer, false, 270.0f );
                transform.GetChild(0).localPosition = new Vector3(-0.1f,0,0);
                break;
            case CardinalDirection.Down:
                this.photonView.RPC( "SetSpriteFlip", PhotonTargets.AllViaServer, true, 270.0f );
                  transform.GetChild(0).localPosition = new Vector3(0.1f,0,0);
                break;
            case CardinalDirection.Left:
                this.photonView.RPC( "SetSpriteFlip", PhotonTargets.AllViaServer, false, 0f );
                  transform.GetChild(0).localPosition = new Vector3(-0.1f,0,0);
                break;
            case CardinalDirection.Right:
                this.photonView.RPC( "SetSpriteFlip", PhotonTargets.AllViaServer, true, 0f );
                  transform.GetChild(0).localPosition = new Vector3(0.1f,0,0);
                break;
            default: break;
        }



    }

    [PunRPC]
    public void rpcSetEquippedTrap(int whattrap)
    {
      if(GetInventory().SelectTrap(whattrap) == true || whattrap == 0)
      {
        GetInventory().equippedTrap = whattrap;
        heldSprite.sprite = gameManager.gameConstants.trapSprites[whattrap];
      }
    }

    [PunRPC]
    public void rpcInteract(Vector3 dir)
    {
      // add .1 for leniency with being out of sync with the server. is this a good idea?
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir,interactDistance + 0.1f);

        if (hit)
        {
          gameManager.scrollingText.NewLine(hit.transform.name);
          if (hit.transform.GetComponent<Door>() != null)
          {
            hit.transform.GetComponent<Door>().SetOpen(!hit.transform.GetComponent<Collider2D>().isTrigger );

          }
          if (hit.transform.GetComponent<HidingSpot>() != null)
          {
            gameManager.photonView.RPC( "OpenHidingSpot", PhotonTargets.AllBufferedViaServer, GetComponent<PhotonView>().ownerId, hit.transform.GetComponent<HidingSpot>().GetPlaceInList() );

          }
        }

    }

    public void TryToPlantTrap(int whattrap)
    {

      RaycastHit2D hit = Physics2D.Raycast(transform.position, CardinalDirectionHelper.ToVector3(facingDirection),interactDistance);

        // if (hit.transform.GetComponent<HidingSpot>() != null)
        if (hit.transform.GetComponent<HidingSpot>() != null)
        {
          gameManager.photonView.RPC( "rpcPlayerSetTrapForHidingSpot", PhotonTargets.AllBufferedViaServer,  photonView.ownerId, hit.transform.GetComponent<HidingSpot>().GetPlaceInList(),whattrap );

        }
    }

    public void TryToInteract()
    {

      RaycastHit2D hit = Physics2D.Raycast(transform.position, CardinalDirectionHelper.ToVector3(facingDirection),interactDistance);

        // if (hit.transform.GetComponent<HidingSpot>() != null)
        if (hit)
        {
          // this.photonView.RPC( "rpcInteract", PhotonTargets.AllViaServer, CardinalDirectionHelper.ToVector3(facingDirection) );
          if (hit.transform.GetComponent<HidingSpot>() != null)
          {
            //if the player has a trap equipped, try to plant it
            if(inventory.equippedTrap != 0)
            {
                  gameManager.photonView.RPC( "rpcPlayerSetTrapForHidingSpot", PhotonTargets.AllBufferedViaServer,  photonView.ownerId, hit.transform.GetComponent<HidingSpot>().GetPlaceInList(), inventory.equippedTrap );
            }
            else//if not holding a trap, try to open the spot
            {
              gameManager.photonView.RPC( "OpenHidingSpot", PhotonTargets.AllBufferedViaServer, GetComponent<PhotonView>().ownerId, hit.transform.GetComponent<HidingSpot>().GetPlaceInList() );
            }



          }
          if (hit.transform.GetComponent<Door>() != null)
          {
            gameManager.photonView.RPC( "OpenDoor", PhotonTargets.AllBufferedViaServer,  hit.transform.GetComponent<Door>().GetPlaceInList() ,true);

          }
        }
    }

    public void OnTriggerStay2D(Collider2D col)
    {
          if ( col.GetComponent<HidingSpot>() != null )
          {
                // if ( Input.GetKeyDown( KeyCode.Space )  )
                // {
                //   gameManager.scrollingText.NewLine("space");
                //     gameManager.photonView.RPC( "OpenHidingSpot", PhotonTargets.AllBufferedViaServer, playerNum, col.GetComponent<HidingSpot>().GetPlaceInList() );
                // }

                 if ( Input.GetKeyDown( KeyCode.Alpha3 )  )
                {
                    gameManager.scrollingText.NewLine("33333");
                    gameManager.photonView.RPC( "rpcPlayerSetTrapForHidingSpot", PhotonTargets.AllBufferedViaServer,  photonView.ownerId, col.GetComponent<HidingSpot>().GetPlaceInList(), 1 );
                }
                 if ( Input.GetKeyDown( KeyCode.Alpha4 )  )
                {
                    gameManager.scrollingText.NewLine("44444");
                    gameManager.photonView.RPC( "rpcPlayerSetTrapForHidingSpot", PhotonTargets.AllBufferedViaServer,  photonView.ownerId, col.GetComponent<HidingSpot>().GetPlaceInList(), 2 );

                }
          }
    }


    public Inventory GetInventory()
    {
      return inventory;
    }
}
