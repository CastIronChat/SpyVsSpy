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
    public GameObject carFollowCam,freeLookCam;
    void Start()
    {
      gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
      transform.parent = gameManager.playerManager.transform;
      if (photonView.isMine)
     {

       // this.photonView.RequestOwnership();
     //   ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
     // hash.Add("lives", -1);

     // PhotonNetwork.player.SetCustomProperties(hash);

     }

    }
	void OnEnable()
	{
     gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
     transform.parent = gameManager.playerManager.transform;
      if (photonView.isMine){

              playerNum = this.photonView.ownerId;
              name = PhotonNetwork.playerName;
              gameManager.localPlayer = playerNum;
              gameManager.photonView.RPC( "PlayerJoinGame", PhotonTargets.AllBufferedViaServer, playerNum , name );
              this.photonView.RPC( "JoinGame", PhotonTargets.AllBufferedViaServer, name, photonView.ownerId  );

      }
	}


    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
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
            // correctPlayerRot = (Quaternion)stream.ReceiveNext();


        }
    }
    [PunRPC]
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
             if(gameManager != null &&  playerNum == num)
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
          transform.parent = gameManager.playerManager.transform;
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
          int count = 0;
          string lifeString = "";
          while(count < lives)
          {
            lifeString = lifeString + "X";
            count ++;
          }
        myScoreCard.transform.GetChild(1).GetComponent<Text>().text = name + " : ";
        myScoreCard.transform.GetChild(3).GetComponent<Text>().text = lifeString;
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
      if (photonView.isMine)
      {
        if(Input.GetMouseButtonDown(1))
        {gameManager.carCamera.ToggleFreeLook();}
      }
    }



}
