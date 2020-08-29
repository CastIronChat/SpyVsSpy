using System.Collections;
using System.Collections.Generic;
using CastIronChat.EntityRegistry;
using UnityEngine;

public class DoorRegistry : Registry<Door>
{
    public DoorRegistry(): base(id: RegistryIds.Doors, name: "doors", validIdsStartAt: 1) {
    }
}
public class Door : MonoBehaviour, Entity
{
    public GameManager gameManager;
    public HidingspotManager doorManager;
    public GameObject doorSprite;

    [Description(@"
        Exit doors can only be opened by a player with all items.
    ")]
    public bool isExit = false;
    // Start is called before the first frame update
    void Start()
    {
      if(doorSprite == null){doorSprite = transform.GetChild(0).gameObject;}
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetOpen(bool open)
    {
      GetComponent<Collider2D>().isTrigger = open;
      doorSprite.SetActive( !open );
    }

    public void OnTriggerExit2D(Collider2D col)
    {
        var player = col.GetComponent<Player>();
        if ( player != null )
        {
            print(uniqueId);
            // col.transform.position = oppositeDoor.position;
            // GetComponent<Collider2D>().isTrigger = false;
            //   doorSprite.SetActive( true );

            if(player.photonView.isMine) {
                gameManager.UpdateVisitedRooms(col.transform.position);
                gameManager.photonView.RPC( "OpenDoor", PhotonTargets.AllBufferedViaServer, uniqueId, false );
            }
        }
    }

    public bool canBeOpenedBy(Player player)
    {
        return !isExit || gameManager.collectibleManager.getState( player ).hasAllCollectibles;
    }

    public int uniqueId { get; set; }
    public BaseRegistry registry { get; set; }
}


