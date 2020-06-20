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
    public float speed;
    public string name;
    public GameObject myScoreCard;
    public Material myColor;
    public List<Color> colors;
    /// TODO support diagonal movement?
    private CardinalDirection movementDirection = CardinalDirection.None;
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerInput input = new PlayerInput();
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

            // this.photonView.RequestOwnership();
            //   ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            // hash.Add("lives", -1);

            // PhotonNetwork.player.SetCustomProperties(hash);

        }

    }
    void OnEnable()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        gameManager = GameObject.Find( "GameManager" ).GetComponent<GameManager>();
        transform.parent = gameManager.playerManager.transform;
        if ( photonView.isMine )
        {

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
            // stream.SendNext(transform.rotation);
        }
        else
        {
            //Network player, receive data
            // score = (int)stream.ReceiveNext();
            // name = (string)stream.ReceiveNext();
            // correctPlayerRot = (Vector3)stream.ReceiveNext();


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
            myScoreCard.transform.GetChild( 1 ).GetComponent<Text>().text = name + " : ";
            myScoreCard.transform.GetChild( 2 ).GetComponent<Text>().text = score.ToString();
        }
    }

    public void ServerUpdateLives(int livesChange)
    {
        lives = livesChange;
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
    public void ServerUpdatePower(int powerChange)
    {
        money = powerChange;
        this.photonView.RPC( "UpdatePower", PhotonTargets.AllBufferedViaServer, money );
    }

    [PunRPC]
    public void UpdatePower(int powerChange)
    {
        money = powerChange;
        if ( myScoreCard != null )
        {
            myScoreCard.transform.GetChild( 1 ).GetComponent<Text>().text = name + " : ";
            myScoreCard.transform.GetChild( 4 ).GetComponent<Text>().text = money.ToString();
        }
    }
    void Update()
    {
        if ( photonView.isMine )
        {
            Move();
        }
    }

    [PunRPC]
    public void SetSpriteFlip(bool flip, float rot)
    {
        GetComponent<SpriteRenderer>().flipX = flip;
        transform.eulerAngles = new Vector3( 0, 0, rot );
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

        // Update velocity
        var movementUnitVector = CardinalDirectionHelper.ToVector3(movementDirection);
        rb.velocity = movementUnitVector * speed * Time.deltaTime;

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

        // float vert = Input.GetAxis("Vertical");
        // float hort = Input.GetAxis("Horizontal");

        // if(vert != 0 && Mathf.Abs(vert) >= Mathf.Abs(hort))
        // {
        //   anim.SetFloat("vert",vert);
        //   anim.SetFloat("hort",0);
        //   rb.velocity = Vector3.up * speed * vert * Time.deltaTime;
        // }else if(hort != 0 && Mathf.Abs(hort) > Mathf.Abs(vert))
        // {
        //   anim.SetFloat("vert",0);
        //   anim.SetFloat("hort",hort);
        //     rb.velocity = Vector3.right * speed * hort * Time.deltaTime;
        // }else
        // {
        //   anim.SetFloat("vert",0);
        //   anim.SetFloat("hort",0);
        //   rb.velocity = Vector3.zero;
        // }

    }
    public void OnTriggerStay2D(Collider2D col)
    {
        if ( Input.GetKeyDown( KeyCode.Space ) && col.transform.tag == "interact" )
        { col.transform.position = col.transform.position + Vector3.up; }

    }

}
