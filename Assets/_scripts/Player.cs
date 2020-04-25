using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player :  Photon.MonoBehaviour
{
  public GameManager gameManager;
    public int numberInList,playerNum,score,lostScore,money;
    public int lives,gamesPlayed,wins;
    public string name;
    public GameObject myScoreCard;
    public Material myColor;
    void Start()
    {
      if (!photonView.isMine)
     {
       // gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
       // transform.parent = gameManager.idleplayerManager.transform;
       // playerNum = photonView.viewID;
       // this.photonView.RequestOwnership();
       print("start");
     }

    }
	void OnEnable()
	{
     gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    transform.parent = gameManager.playerManager.transform;
      if (photonView.isMine){

              playerNum = photonView.viewID;
              name = PhotonNetwork.playerName;
                gameManager.photonView.RPC( "PlayerJoinGame", PhotonTargets.AllBufferedViaServer, photonView.viewID , name );
                // this.photonView.RPC( "JoinGame", PhotonTargets.AllBufferedViaServer, name, photonView.viewID  );

                gameManager.localPlayer = photonView.viewID;
      }
	}

    void Awake()
    {
      // gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
     // transform.parent = gameManager.idleplayerManager.transform;
         if (photonView.isMine)
        {
          // gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
          // playerNum = photonView.viewID;
          // this.photonView.RequestOwnership();
          print("Awake");
        }
        else
        {

        }


    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
          //  stream.SendNext(score);
          //  stream.SendNext(name);
            // stream.SendNext(transform.rotation);
        }
        else
        {
            //Network player, receive data
        //  score = (int)stream.ReceiveNext();
        //    name = (string)stream.ReceiveNext();
            // correctPlayerRot = (Quaternion)stream.ReceiveNext();


        }
    }
    public void StartMyTurn(int num)
    {
      if (photonView.isMine)
      {
              if (gameManager == null)
             {
                // playerNum = photonView.viewID;
               gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

             }else
             {


             }
             gameManager.car.photonView.RequestOwnership();
             if(gameManager != null && (num == numberInList || playerNum == num))
             {
               // gameManager.car.GetComponent<PhotonView>().RequestOwnership();

             }
      }
    }

    [PunRPC]
    public void JoinGame(string newname, int photonNumber  )
    {
        name = newname;
        playerNum = photonNumber;
          numberInList = -1;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
       transform.parent = gameManager.idleplayerManager.transform;
    }

    [PunRPC]
    public void SetNumberInList(int listPlace  )
    {
        numberInList = listPlace;
    }

    public void ServerUpdateScore(int scoreChange  )
    {
          score = scoreChange;
          this.photonView.RPC( "UpdateScore", PhotonTargets.AllBufferedViaServer, score  );
    }

    [PunRPC]
    public void UpdateScore(int scoreChange  )
    {
        score = scoreChange;
          if(myScoreCard != null){
            myScoreCard.active = true;
        myScoreCard.transform.GetChild(1).GetComponent<Text>().text = name + " : ";
        myScoreCard.transform.GetChild(2).GetComponent<Text>().text = score.ToString() ;
      }
    }

    public void ServerUpdateLives(int livesChange  )
    {
        lives = livesChange;
          this.photonView.RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer, lives  );
    }

    [PunRPC]
    public void UpdateLives(int livesChange  )
    {
        lives = livesChange;
        if(myScoreCard != null){
        myScoreCard.transform.GetChild(1).GetComponent<Text>().text = name + " : ";
        myScoreCard.transform.GetChild(3).GetComponent<Text>().text = lives.ToString();
      }
    }
    public void ServerUpdatePower(int powerChange  )
    {
        money = powerChange;
          this.photonView.RPC( "UpdatePower", PhotonTargets.AllBufferedViaServer, money  );
    }

    [PunRPC]
    public void UpdatePower(int powerChange  )
    {
        money = powerChange;
        if(myScoreCard != null){
        myScoreCard.transform.GetChild(1).GetComponent<Text>().text = name + " : ";
        myScoreCard.transform.GetChild(4).GetComponent<Text>().text = money.ToString();
      }
    }
    void Update()
    {
      // if(transform.parent == null)
      // {
      //   if (gameManager == null)
      //  {
      //     // playerNum = photonView.viewID;
      //    gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
      //
      //  }
      //  transform.parent = gameManager.idleplayerManager;
      // }
    }



}
