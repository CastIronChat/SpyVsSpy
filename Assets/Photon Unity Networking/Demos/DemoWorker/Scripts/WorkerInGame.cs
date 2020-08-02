// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerInGame.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;

public class WorkerInGame : Photon.MonoBehaviour
{
    public Transform playerPrefab,spawnSpaceParent;

    public void Awake()
    {
        // in case we started this demo with the wrong scene being active, simply load the menu scene
        if (!PhotonNetwork.connected)
        {
            BackToMenu();
            return;
        }

        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
        GameObject clone = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnSpaceParent.GetChild(PhotonNetwork.playerList.Length).position, Quaternion.identity, 0) as GameObject;
        //clone.transform.position = spawnSpaceParent.GetChild(clone.GetComponent<PhotonView>().ownerId).position;
        print(this.photonView.ownerId);
    }

    // public void OnGUI()
    // {
    //     if (GUILayout.Button("Return to Lobby"))
    //     {
    //         PhotonNetwork.LeaveRoom();  // we will load the menu level when we successfully left the room
    //     }
    // }

    public void OnMasterClientSwitched(PhotonPlayer player)
    {
        Debug.Log("OnMasterClientSwitched: " + player);

        // InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message

        // if (chatComponent != null)
        // {
        //     // to check if this client is the new master...
        //     if (player.IsLocal)
        //     {
        //         message = "You are Master Client now.";
        //     }
        //     else
        //     {
        //         message = player.NickName + " is Master Client now.";
        //     }
        //
        //
        //     chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
        // }
    }

    public void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom (local)");

        BackToMenu();
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("OnDisconnectedFromPhoton");

        BackToMenu();
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log("OnPhotonInstantiate " + info.sender);    // you could use this info to store this or react
    }

    public void OnPhotonPlayerConnected(PhotonPlayer player)
    {
        Debug.Log("OnPhotonPlayerConnected: " + player);
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log("OnPlayerDisconneced: " + player);
    }

    public void OnFailedToConnectToPhoton()
    {
        Debug.Log("OnFailedToConnectToPhoton");

        BackToMenu();
    }

    private void BackToMenu() {
        // For development, menu will return you to whichever scene you have open now.
        WorkerMenu.SceneNameGame = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(WorkerMenu.SceneNameMenu);
    }
}
