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
    public float speed,interactDistance = 0.4f;
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
    void Start()
    {
        if ( this.photonView.ownerId < colors.Count )
        { GetComponent<SpriteRenderer>().color = colors[this.photonView.ownerId]; }

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find( "GameManager" ).GetComponent<GameManager>();
        transform.parent = gameManager.playerManager.transform;
        if ( photonView.isMine )
        {


        }

    }
    void OnEnable()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        inventory = new Inventory();
        inventory.traps = new List<int>();
        inventory.traps.Add(0);
        inventory.traps.Add(1);
        inventory.traps.Add(2);
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

    [PunRPC]
    public void UpdateTraps(int trapUsed)
    {
        inventory.UseTrap(trapUsed);
        if ( myScoreCard != null )
        {
            myScoreCard.active = true;
            myScoreCard.transform.GetChild( 1 ).GetComponent<Text>().text = name + " : ";
            myScoreCard.transform.GetChild( 2 ).GetComponent<Text>().text = inventory.GetTrapsString();
        }
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
          if(Vector3.Distance(serverPos,transform.position) > 0.1f)
          {
            transform.position = Vector3.MoveTowards(transform.position, serverPos, speed  *  Time.deltaTime);
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
        rb.velocity = newvel * speed * Time.deltaTime;
    }

    public void UseTraps()
    {
      if(Input.GetKeyDown(KeyCode.Alpha1) && inventory.SelectTrap(1))
      {inventory.equippedTrap = 1; gameManager.scrollingText.NewLine("equiped trap 1");}
        if(Input.GetKeyDown(KeyCode.Alpha2) && inventory.SelectTrap(2))
        {inventory.equippedTrap = 2; gameManager.scrollingText.NewLine("equiped trap 2");}
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
                break;
            case CardinalDirection.Down:
                this.photonView.RPC( "SetSpriteFlip", PhotonTargets.AllViaServer, false, 270.0f );
                break;
            case CardinalDirection.Left:
                this.photonView.RPC( "SetSpriteFlip", PhotonTargets.AllViaServer, false, 0f );
                break;
            case CardinalDirection.Right:
                this.photonView.RPC( "SetSpriteFlip", PhotonTargets.AllViaServer, true, 0f );
                break;
            default: break;
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
            gameManager.photonView.RPC( "OpenHidingSpot", PhotonTargets.AllBufferedViaServer, GetComponent<PhotonView>().ownerId, hit.transform.GetComponent<HidingSpot>().GetPlaceInList() );

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
