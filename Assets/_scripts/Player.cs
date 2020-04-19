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
        // gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // playerNum = photonView.viewID;
        // this.photonView.RequestOwnership();
        // playerNum = photonView.viewID;
        name = PhotonNetwork.playerName;
        // name = playerNum.ToString() + " x " + playerNum.ToString();
        lives = gameManager.startingLives;
        this.photonView.RPC( "UpdateLives", PhotonTargets.AllBufferedViaServer,lives );
          gameManager.photonView.RPC( "PlayerJoinGame", PhotonTargets.AllBufferedViaServer, photonView.viewID , name );
          gameManager.localPlayer = photonView.viewID;
      }
	}

    void Awake()
    {

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
            stream.SendNext(score);
            stream.SendNext(name);
            // stream.SendNext(transform.rotation);
        }
        else
        {
            //Network player, receive data
          score = (int)stream.ReceiveNext();
            name = (string)stream.ReceiveNext();
            // correctPlayerRot = (Quaternion)stream.ReceiveNext();


        }
    }
    public void StartMyTurn(int num)
    {
      money++;
      if (photonView.isMine)
      {
              if (gameManager == null)
             {
                // playerNum = photonView.viewID;
               gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

             }else
             {


             }
             if(gameManager != null && (num == numberInList || playerNum == num))
             {
               // gameManager.car.GetComponent<PhotonView>().RequestOwnership();
               gameManager.car.photonView.RequestOwnership();
             }
      }
    }
    [PunRPC]
    public void SetNumberInList(int listPlace  )
    {
        numberInList = listPlace;
    }
    [PunRPC]
    public void UpdateScore(int scoreChange  )
    {
        score = scoreChange;
          if(myScoreCard != null){
        myScoreCard.transform.GetChild(1).GetComponent<Text>().text = name + " : ";
        myScoreCard.transform.GetChild(2).GetComponent<Text>().text = score.ToString() + ":L-" + lives.ToString();
      }
    }
    [PunRPC]
    public void UpdateLives(int livesChange  )
    {
        lives = livesChange;
        if(myScoreCard != null){
        myScoreCard.transform.GetChild(1).GetComponent<Text>().text = name + " : ";
        myScoreCard.transform.GetChild(2).GetComponent<Text>().text = score.ToString() + ":L-" + lives.ToString();
      }
    }
    void Update()
    {

    }



}
