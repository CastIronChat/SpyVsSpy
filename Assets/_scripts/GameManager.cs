using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : Photon.MonoBehaviour
{
  public PlayerManager playerManager;
  public CarControls car;
    public GameObject playerPrefab;
  public GameObject scoreBoard;
  public int activePlayers;
  public int currentPlayer = -1,currentTurn = -1;//turn is 0-x and player is their photonid
  public Renderer myRenderer;
  public List<Material> colors;
    public void Awake()
    {
        if (!PhotonNetwork.connected)
        {

        }
        // GameObject clone = PhotonNetwork.Instantiate(this.playerPrefab.name, transform.position, Quaternion.identity, 0);
        // clone.GetComponent<Player>().gameManager = GetComponent<GameManager>();
    }
    [PunRPC]
    public void NextTurn(int setTurn)
    {
        currentPlayer = playerManager.transform.GetChild(setTurn).GetComponent<Player>().playerNum;



        myRenderer.material = colors[setTurn];
        foreach(Transform go in playerManager.transform)
        {
          if(go.GetComponent<Player>().numberInList == setTurn)
          {go.GetComponent<Player>().StartMyTurn(setTurn);}
        }

    }
    [PunRPC]
    public void PlayerJoinGame(int newPlayer,string newname  )
    {
        print("new player Number: " + newPlayer);
        PopulateScoreBoard(newPlayer,newname);
        // GameObject clone = Instantiate(playerScoreCard,new Vector3(playerScoreCard.transform.position.x,playerScoreCard.transform.position.y - 35,playerScoreCard.transform.position.z),playerScoreCard.transform.rotation);
        // clone.transform.parent = playerScoreCard.transform.parent;
        // clone.transform.GetChild(1).GetComponent<Text>().text = "Player" + newPlayer.ToString();
        // clone.transform.GetChild(2).GetComponent<Text>().text = newPlayer.ToString();
    }

    public void PopulateScoreBoard(int newPlayer,string newname )
    {
      int cardcount = 0;
      int playercount = 0;

      Transform nextPlayerInOrder = playerManager.transform.GetChild(0);
      if(nextPlayerInOrder.GetComponent<Player>().playerNum <= 0){
        nextPlayerInOrder.GetComponent<Player>().playerNum = nextPlayerInOrder.GetComponent<PhotonView>().viewID;

      }
      int lowestPlayerNum = 99999;
      int lastPlayerNum = -1;
        while(playercount < playerManager.transform.childCount)
        {


            foreach(Transform go in playerManager.transform)
            {
              //So the new players has their name when they join
              if(go.GetComponent<Player>().name.Length <= 0){
                go.GetComponent<Player>().name = newname;

              }
              if(go.GetComponent<Player>().playerNum <= 0){
                go.GetComponent<Player>().playerNum = go.GetComponent<PhotonView>().viewID;

              }
              if(go.GetComponent<Player>().playerNum > lastPlayerNum && go.GetComponent<Player>().playerNum < lowestPlayerNum )
              {
                nextPlayerInOrder = go;
                lowestPlayerNum = go.GetComponent<Player>().playerNum;
              }

            }
            lastPlayerNum =  lowestPlayerNum;
            lowestPlayerNum = 99999;

            scoreBoard.transform.GetChild(playercount).gameObject.active = true;
            scoreBoard.transform.GetChild(playercount).GetChild(1).GetComponent<Text>().text = nextPlayerInOrder.GetComponent<Player>().name + " : ";
            scoreBoard.transform.GetChild(playercount).GetChild(2).GetComponent<Text>().text = nextPlayerInOrder.GetComponent<Player>().playerNum.ToString();

            nextPlayerInOrder.GetComponent<Player>().myScoreCard = scoreBoard.transform.GetChild(playercount).gameObject;
            nextPlayerInOrder.parent = playerManager.transform;
            nextPlayerInOrder.GetComponent<Player>().photonView.RPC( "SetNumberInList", PhotonTargets.AllBufferedViaServer, playercount );


            scoreBoard.transform.GetChild(playercount).GetChild(3).GetComponent<RawImage>().color = colors[playercount].color;
            playercount++;
            if(playercount >= scoreBoard.transform.childCount){return;}
       }
       while(playercount < scoreBoard.transform.childCount)
       {
         scoreBoard.transform.GetChild(playercount).gameObject.active = false;
           playercount++;
       }

    }
     void Update()
    {
      if( PhotonNetwork.isMasterClient  && Input.GetKeyDown(KeyCode.Space) )
      {

        currentTurn = currentTurn + 1;
        if(currentTurn >= playerManager.transform.childCount){currentTurn = 0;}
        myRenderer.material = colors[currentTurn];
        this.photonView.RPC( "NextTurn", PhotonTargets.AllBufferedViaServer, currentTurn );

      }
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

          if (stream.isWriting)
          {

              stream.SendNext(currentTurn);

          }
          else
          {

            currentTurn = (int)stream.ReceiveNext();



          }

    }

    [PunRPC]
    public void HitSomething( int whichPlayer )
    {
      if(whichPlayer < 0){whichPlayer = 0;}

      scoreBoard.transform.GetChild(whichPlayer).GetChild(2).GetComponent<Text>().text = (Convert.ToInt32(scoreBoard.transform.GetChild(whichPlayer).GetChild(2).GetComponent<Text>().text) + 1).ToString();

    }

    public void OnGUI()
    {
        if (GUILayout.Button("Return to Lobby"))
        {
            PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
        }
    }

    public void OnMasterClientSwitched(PhotonPlayer player)
    {
        Debug.Log("XXXXXXOnMasterClientSwitched: " + player);


    }

    public void OnLeftRoom()
    {
        Debug.Log("XXXXXXOnLeftRoom (local)");

        // back to main menu
        // SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("XXXXOnDisconnectedFromPhoton");

        // back to main menu
        // SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("XXXXXOnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("XXXXXXOnPhotonPlayerConnected: " + player);
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("XXXXXXXXXOnPlayerDisconneced: " + player);
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("XXXXXXXXXXOnFailedToConnectToPhoton");

        // back to main menu
        // SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
    }
}
