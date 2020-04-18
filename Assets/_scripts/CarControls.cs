using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using System.Collections;

public class CarControls : Photon.MonoBehaviour
{
  public GameManager gameManager;
    ThirdPersonCamera cameraScript;
    ThirdPersonController controllerScript;
    public float acceleration;
    public int drivingPlayer = 1;
	bool firstTake = false;
  private Rigidbody rb;
  public List<Material> colors;
	void OnEnable()
	{
	 	firstTake = true;
    rb = GetComponent<Rigidbody>();
    if (photonView.isMine)
   {rb.isKinematic = false;}
	}

    void Awake()
    {
      rb = GetComponent<Rigidbody>();
      if (photonView.isMine)
     {rb.isKinematic = false;}
        // cameraScript = GetComponent<ThirdPersonCamera>();
        // controllerScript = GetComponent<ThirdPersonController>();
        //
        //  if (photonView.isMine)
        // {
        //     //MINE: local player, simply enable the local scripts
        //     cameraScript.enabled = true;
        //     controllerScript.enabled = true;
        // }
        // else
        // {
        //     cameraScript.enabled = false;
        //
        //     controllerScript.enabled = true;
        //     controllerScript.isControllable = false;
        // }

        gameObject.name = gameObject.name + photonView.viewID;
    }

    // void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    // {
    //     if (stream.isWriting)
    //     {
    //         //We own this player: send the others our data
    //         stream.SendNext( (float)acceleration);
    //         stream.SendNext( (int)lastPlayerActing);
    //     }
    //     else
    //     {
    //         //Network player, receive data
    //         acceleration = (float)stream.ReceiveNext();
    //         lastPlayerActing = (int)stream.ReceiveNext();
    //
    //     }
    // }

    private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
    private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

    void Update()
    {
      // transform.position = Vector3.Lerp(transform.position, transform.position + transform.forward, Time.deltaTime * acceleration);

      if (!photonView.isMine)
      {
        rb.isKinematic = true;
          //Update remote player (smooth this, this looks good, at the cost of some accuracy)
        transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 15  );
        // transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 15 / (0.1f + Vector3.Distance(transform.position,correctPlayerPos)) );
          transform.rotation = Quaternion.Lerp(  transform.rotation, correctPlayerRot, Time.deltaTime * 15);


          // if( Input.GetKeyDown(KeyCode.Space) && this.photonView.ownerId != PhotonNetwork.player.ID )
          // {
          //   Vector3 colVector = new Vector3( Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ) );
          //   this.photonView.RPC( "ColorRpc", PhotonTargets.AllBufferedViaServer, colVector );
          //       this.photonView.RequestOwnership();
          // }



      }else
      {

        rb.isKinematic = false;
          Drive();
          // this.photonView.RPC( "Accelerate", PhotonTargets.AllBufferedViaServer,1 );
            Debug.Log( "acceleration = 0.5f" + PhotonNetwork.player.ID.ToString());

            if(Input.GetMouseButtonDown(0)){rb.AddForce(Vector3.up * 2500,ForceMode.Impulse);}
      }





    }
    [PunRPC]
    public void Accelerate(int fromPlayer  )
    {
        print("XXXceleration");
    }
    public void Drive()
    {
      GetComponent<CarControlCS>().ControlCar();

        if(Input.GetKey(KeyCode.D))
        {

        }
    }

    [PunRPC]
    public void ColorRpc( Vector3 col )
    {
      rb.isKinematic = true;
      // transform.position = correctPlayerPos;
      //   transform.rotation = correctPlayerRot;
      //   correctPlayerPos = transform.position;
      //   correctPlayerRot =   transform.rotation;
        rb.isKinematic = false;
        Color color = new Color( col.x, col.y, col.z );
        this.gameObject.GetComponent<Renderer>().material.color = color;
    }

    public void OnCollisionEnter(Collision col)
    {
        gameManager.photonView.RPC( "HitSomething", PhotonTargets.AllBufferedViaServer,  col.relativeVelocity.magnitude);
      // if( PhotonNetwork.isMasterClient  && col.relativeVelocity.magnitude > 5 )
      // {
      //   gameManager.photonView.RPC( "HitSomething", PhotonTargets.AllBufferedViaServer,   col.relativeVelocity.magnitude);
      //
      // }
    }
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data

            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            correctPlayerPos = transform.position;
            correctPlayerRot = transform.rotation;
        }
        else
        {
            //Network player, receive data
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();

        }
    }
}
