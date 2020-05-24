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
    public float acceleration,distToGround = 0.5f,jumpheight = 15.0f,airControlPower = 3.0f;
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
      //When not driving, lerp to real position, and remain kinematic to not register impacts which are handled by the master server
      if (!photonView.isMine)
      {
        rb.isKinematic = true;
        transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);//(115 * Vector3.Distance(correctPlayerPos, transform.position))  );
        transform.rotation = Quaternion.Lerp(  transform.rotation, correctPlayerRot, Time.deltaTime * 5);


      }


    }

    public void DriveCar()
    {


        rb.isKinematic = false;
          Drive();
          // this.photonView.RPC( "Accelerate", PhotonTargets.AllBufferedViaServer,1 );
            Debug.Log( "Driving: " + PhotonNetwork.player.ID.ToString());
            bool onGround = IsGrounded();
            if(Input.GetMouseButtonDown(2) && onGround){
              // rb.AddForce(Vector3.up * 2500,ForceMode.Impulse);
              rb.velocity = (rb.velocity +  (transform.forward * jumpheight * 0.2f) + (transform.up * jumpheight));

            }
            if(onGround == false)
            {
              AirControl();
            }


    }

      public bool IsGrounded()
   {
     return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
   }
   public void AirControl()
   {
//+ transform.right * (Input.GetAxis("Vertical")  * airControlPower)
     transform.Rotate(transform.forward * (-Input.GetAxis("Horizontal") * airControlPower) );
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
      if(col.gameObject.tag != "Hazard")
      {
        gameManager.photonView.RPC( "HitSomething", PhotonTargets.AllBufferedViaServer,  col.relativeVelocity.magnitude);
      }

      // if( PhotonNetwork.isMasterClient  && col.relativeVelocity.magnitude > 5 )
      // {
      //   gameManager.photonView.RPC( "HitSomething", PhotonTargets.AllBufferedViaServer,   col.relativeVelocity.magnitude);
      //
      // }
    }


//Check requests so that when players join the game they dont
    public void OnOwnershipRequest(object[] viewAndPlayer)
  {
    PhotonView view = viewAndPlayer[0] as PhotonView;
    PhotonPlayer requestingPlayer = viewAndPlayer[1] as PhotonPlayer;

    Debug.Log("OnOwnershipRequest(): Player " + requestingPlayer + " requests ownership of: " + view + ".");
    print(">>>> " + requestingPlayer.ID.ToString());
    if(gameManager.partOfTurn != 0  )
      {
          print(requestingPlayer.ToString() + " >>>> GIVING TO >>>> " + requestingPlayer.ID.ToString());
                view.TransferOwnership(requestingPlayer.ID);

    }
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
