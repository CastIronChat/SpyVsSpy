using System;
using System.Collections;
using System.Collections.Generic;
using CastIronChat.EntityRegistry;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRegistry : Registry<Player>
{
    // Never allocate IDs because we are using values provided by Photon for this (ownerId at present, possibly viewID in the future)
    public PlayerRegistry() : base(id: RegistryIds.Player, name: "players", validIdsStartAt: 1, idAllocationMode: IdAllocationMode.Never) {}
}
public class Player : Photon.MonoBehaviour, Entity
{
    public GameManager gameManager
    {
        get => GameManager.instance;
    }

    public GameConstants gameConstants
    {
        get => gameManager.gameConstants;
    }

    public PlayerManager playerManager
    {
        get => gameManager.playerManager;
    }

    /// <summary>
    /// Unique ID to identify this player over the network.
    /// NOTE using ownerID will not work if we want to support 2x players on one client.
    /// We can switch photon ViewID instead.
    /// </summary>
    public int playerId
    {
        get => photonView.ownerId;
    }

    /// <summary>
    /// Sometimes we want players to be numbered starting from 0, without any gaps.
    /// This technically does not follow the rules of playerId.  If one player leaves and another
    /// enters, the new player may reuse the index of the leaving player, but playerId will be unique for both.
    ///
    /// Exposing as a second property so that we can make these numbers different in the future without crazy code changes.
    /// </summary>
    public int playerIndex
    {
        get => playerId;
    }
    public int score, lostScore, money;
    public int lives, gamesPlayed, wins;
    public float speed,networkspeed = 15.0f,interactDistance = 0.4f,attackInputLock = 0.5f,inputLockTimer;
    new public string name;
    public GameObject myScoreCard;
    public Material myColor;
    public List<Color> colors;
    public SpriteRenderer characterSprite,heldSprite,weaponSprite;

    /// TODO support diagonal movement?
    private CardinalDirection movementDirection = CardinalDirection.None,facingDirection = CardinalDirection.None;
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerInput input = new PlayerInput();
    private Inventory inventory;
    private Vector3 serverPos;
    private Quaternion serverRot;


    private float iFrames, poisonTimer; //invincibility frames
    private bool isPoisoned; //after a time defined on the gameconstants the player takes damage from poison, clear poison by leaving the room...or opening a window?
    public int uniqueId
    {
        get => playerId;
        set => throw new Exception("not supported");
    }
    public BaseRegistry registry { get; set; }

    void Start()
    {
        if ( playerIndex < colors.Count )
        { characterSprite.GetComponent<SpriteRenderer>().color = colors[this.playerIndex]; }

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if(heldSprite == null)
        {heldSprite = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();}

        if ( photonView.isMine  )
        {

            rb.isKinematic = false;
        }

    }
    void OnEnable()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        inventory = new Inventory();
        inventory.traps[gameConstants.trapTypes[0]] = 1;
        inventory.traps[gameConstants.trapTypes[1]] = 1;
        inventory.traps[gameConstants.trapTypes[2]] = 1;
        inventory.traps[gameConstants.trapTypes[3]] = 1;
        heldSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();

        playerManager.add( this );
        if ( photonView.isMine )
        {
            name = PhotonNetwork.playerName;
            gameManager.photonView.RPC( "PlayerJoinGame", PhotonTargets.AllBufferedViaServer, playerId, name );
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
            myScoreCard.SetActive( true );

        }
    }

    /// Modify HUD UI to match player's inventory
    public void UpdateTrapHUD( )
    {
        if ( myScoreCard != null )
        {
            myScoreCard.SetActive( true );
            myScoreCard.transform.GetChild( 1 ).GetComponent<Text>().text = name + " : ";
            myScoreCard.transform.GetChild( 2 ).GetComponent<Text>().text = inventory.GetTrapsString();
        }

        if ( photonView.isMine )
        {
          //update the local players inventory visuals for what traps they have
          inventory.UpdateInventoryHud();
        }

    }

    [PunRPC]
    public void AddorRemoveTrap(TrapType trapType,int setto)
    {
        inventory.SetTrapCount(trapType, setto);
    }

    [PunRPC]
    public void RemoveCollectible(int whichCollectible,int setto)
    {
        var collectible = gameConstants.collectibleTypes[whichCollectible];
        inventory.RemoveCollectible(collectible);
    }

    [PunRPC]
    public void AddCollectible(int whichCollectible,int setto)
    {
        var collectible = gameConstants.collectibleTypes[whichCollectible];
        inventory.AddCollectible(collectible,setto);
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
      //after taking damage to avoid double taps from the same source, add invincibility frames
      if(iFrames > 0){iFrames -= Time.deltaTime;}

        if ( photonView.isMine )
        {

            if(isPoisoned == true)
            {
                poisonTimer += Time.deltaTime;
                if(poisonTimer >= gameConstants.poisonTime)
                {
                  isPoisoned = false;
                  poisonTimer = -1;
                  this.photonView.RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer, lives - 1 );
                }
            }

          //to prevent weapon spam and multi directional attack, have a brief input lock after attacking
          if(inputLockTimer > 0){inputLockTimer -= Time.deltaTime;}
          else
          {
              if(input.__debugInventoryResetDown())
              {
                inventory.traps[gameConstants.trapTypes[0]] = 0;
                inventory.traps[gameConstants.trapTypes[1]] = 1;
                inventory.traps[gameConstants.trapTypes[2]] = 1;
                inventory.traps[gameConstants.trapTypes[3]] = 1;
              }
                Move();
                UseTraps();
                if ( Input.GetKeyDown(KeyCode.Space)){TryToInteract();}
                // if ( input.GetInteractDown() ){TryToInteract();}
          }
        }
        else
        {
          transform.rotation = serverRot;
          if(Vector3.Distance(serverPos,transform.position) > 0.01f)
          {
            transform.position = Vector3.Lerp(transform.position, serverPos, Time.deltaTime * networkspeed);
          }

        }
        // Try doing this every frame.  It's so much easier to think about this way.
        UpdateTrapHUD();
    }



    [PunRPC]
    public void SetSpriteFlip(bool flip, float rot)
    {
        characterSprite.GetComponent<SpriteRenderer>().flipX = flip;
        characterSprite.transform.eulerAngles = new Vector3( 0, 0, rot );
    }
    [PunRPC]
    public void SetVelocity(Vector3 newvel)
    {
        // rb.velocity = newvel * speed * Time.deltaTime;
    }

    public void UseTraps()
    {
        if(inventory.traps.Count == 0) return;
        int? trapIndexToEquip = null;
        var trapSelectionPressed = input.getChooseTrapByIndexDown();
        if(trapSelectionPressed.HasValue) {
            var trapIndexPressed = trapSelectionPressed.Value;
            if(trapIndexPressed <= inventory.traps.Count) {
                trapIndexToEquip = trapIndexPressed;
            }
        }
        if(input.GetChooseNextTrapDown()) {
            trapIndexToEquip = inventory.equippedTrapIndex + 1;
            if(trapIndexToEquip > inventory.traps.Count) {
                trapIndexToEquip = 0;
            }
        }
        if(input.GetChoosePreviousTrapDown()) {
            trapIndexToEquip = inventory.equippedTrapIndex - 1;
            if(trapIndexToEquip < 0) {
                trapIndexToEquip = inventory.traps.Count - 1;
            }
        }
        if(trapIndexToEquip.HasValue) {
            var trap = gameConstants.trapTypes[trapIndexToEquip.Value];
            photonView.RPC( "rpcSetEquippedTrap", PhotonTargets.AllBufferedViaServer, trap);
        }

        // Using traps is handled by interaction code elsewhere.
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
                // transform.GetChild(0).localPosition = new Vector3(-0.1f,0,0);
                break;
            case CardinalDirection.Down:
                this.photonView.RPC( "SetSpriteFlip", PhotonTargets.AllViaServer, true, 270.0f );
                  // transform.GetChild(0).localPosition = new Vector3(0.1f,0,0);
                break;
            case CardinalDirection.Left:
                this.photonView.RPC( "SetSpriteFlip", PhotonTargets.AllViaServer, false, 0f );
                  // transform.GetChild(0).localPosition = new Vector3(-0.1f,0,0);
                break;
            case CardinalDirection.Right:
                this.photonView.RPC( "SetSpriteFlip", PhotonTargets.AllViaServer, true, 0f );
                  // transform.GetChild(0).localPosition = new Vector3(0.1f,0,0);
                break;
            default: break;
        }
    }

    [PunRPC]
    public void rpcSetEquippedTrap(TrapType trapType)
    {
      if(GetInventory().HasTrap(trapType) == true)
      {
        heldSprite.enabled = true;
        GetInventory().equippedTrap = trapType;
        heldSprite.sprite = trapType.sprite;
        gameManager.scrollingText.NewLine("Equipped trap #" + trapType.uniqueId + ": " + trapType.name);
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
            gameManager.photonView.RPC( "OpenHidingSpot", PhotonTargets.AllBufferedViaServer, playerId, hit.transform.GetComponent<HidingSpot>().GetPlaceInList() );

          }
        }

    }

    [PunRPC]
    public void rpcGetThrownByTrap(Vector3 dir,float lockout)
    {
      if(photonView.isMine == true)
      {
          GetComponent<Rigidbody2D>().velocity = dir;
          inputLockTimer = lockout;
      }
    }

    [PunRPC]
    public void rpcAttackAnimation()
    {
      inputLockTimer = attackInputLock;
      rb.velocity = Vector3.zero;
        if(characterSprite.GetComponent<SpriteRenderer>().flipX == false)
        {
          if(GetInventory().hasBriefcase == true)
          {
            anim.Play("punch");
          }
          else
          {
            anim.Play("stab");
          }
        }
        else
        {
          if(GetInventory().hasBriefcase == true)
          {
            anim.Play("reversepunch");
          }
          else
          {
            anim.Play("reversestab");
          }
        }

    }

    public void TryToPlantTrap(TrapType whattrap)
    {

      RaycastHit2D hit = Physics2D.Raycast(transform.position, CardinalDirectionHelper.ToVector3(facingDirection),interactDistance);

        if (hit.transform.GetComponent<HidingSpot>() != null)
        {
          gameManager.photonView.RPC( "rpcPlayerSetTrapForHidingSpot", PhotonTargets.AllBufferedViaServer,  playerId, hit.transform.GetComponent<HidingSpot>().GetPlaceInList(),whattrap );

        }
    }

    public void TryToInteract()
    {

      RaycastHit2D hit = Physics2D.Raycast(transform.position, CardinalDirectionHelper.ToVector3(facingDirection),interactDistance);


        if (hit && (hit.transform.GetComponent<HidingSpot>() != null || hit.transform.GetComponent<Door>() != null))
        {
          // this.photonView.RPC( "rpcInteract", PhotonTargets.AllViaServer, CardinalDirectionHelper.ToVector3(facingDirection) );
          if (hit.transform.GetComponent<HidingSpot>() != null)
          {
            //if the player has a trap equipped, try to plant it
            if(inventory.equippedTrap.isUsable && inventory.HasTrap(inventory.equippedTrap))
            {
                  gameManager.photonView.RPC( "rpcPlayerSetTrapForHidingSpot", PhotonTargets.AllBufferedViaServer,  playerId, hit.transform.GetComponent<HidingSpot>().GetPlaceInList(), inventory.equippedTrap );
            }
            else//if not holding a trap, try to open the spot
            {
              gameManager.photonView.RPC( "OpenHidingSpot", PhotonTargets.AllBufferedViaServer, playerId, hit.transform.GetComponent<HidingSpot>().GetPlaceInList() );
            }



          }

          var door = hit.transform.GetComponent<Door>();
          if (door != null)
          {
              if ( door.canBeOpenedBy( this ) )
              {
                  gameManager.photonView.RPC( "OpenDoor", PhotonTargets.AllBufferedViaServer, door.uniqueId, true );
              }

          }
        }
        else
        {
          //if a hiding spot is not the closest object facing the player, they attack
              this.photonView.RPC( "rpcAttackAnimation", PhotonTargets.AllViaServer);
        }
    }


    public void BubbleAnimation()
    {
          if(photonView.isMine)
          {
              gameManager.EnableBubbles();
          }

    }


    public void OnTriggerEnter2D(Collider2D col)
    {
        //the server checks when a weapon triggers on a player character, and that the weapon is not their own
        if ( PhotonNetwork.isMasterClient && col.transform.parent != this.transform)
        {

          //TODO: differentiate attack types
            //after taking damage to avoid double taps from the same source, add invincibility frames
          if(col.gameObject.tag == "Weapon")
          {
            iFrames = 0.5f;
            this.photonView.RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer, lives - 2 );
          }
          if(col.gameObject.tag == "Punch")
          {
            iFrames = 0.5f;
            this.photonView.RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer, lives - 1 );
          }


        }

    }


      void OnParticleCollision(GameObject other)
      {
        if(other.transform.tag == "Poison")
        {
          if(isPoisoned == false)
          {
            isPoisoned = true;
            poisonTimer = 0;
            gameManager.scrollingText.NewLine("GASP POISON " );
          }

        }
      }


    public Inventory GetInventory()
    {
      return inventory;
    }
}
