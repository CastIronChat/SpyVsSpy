﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using System.Collections;

public class CarControls : Photon.MonoBehaviour
{
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
	}

    void Awake()
    {
      rb = GetComponent<Rigidbody>();
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
          //Update remote player (smooth this, this looks good, at the cost of some accuracy)
        transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 15);
          transform.rotation = Quaternion.Lerp(  transform.rotation, correctPlayerRot, Time.deltaTime * 15);

          if( Input.GetKeyDown(KeyCode.Space) && this.photonView.ownerId != PhotonNetwork.player.ID )
          {
            Vector3 colVector = new Vector3( Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ), Random.Range( 0.0f, 1.0f ) );
            this.photonView.RPC( "ColorRpc", PhotonTargets.AllBufferedViaServer, colVector );
                this.photonView.RequestOwnership();
          }



      }else
      {


          Drive();
          // this.photonView.RPC( "Accelerate", PhotonTargets.AllBufferedViaServer,1 );
            Debug.Log( "acceleration = 0.5f" + PhotonNetwork.player.ID.ToString());


      }





    }
    [PunRPC]
    public void Accelerate(int fromPlayer  )
    {
        print("XXXceleration");
    }
    public void Drive()
    {
      if(Input.GetMouseButton(0))
      {
        acceleration = 520.5f;
        rb.AddForce(transform.forward * acceleration * Time.deltaTime);
        // this.photonView.RPC( "Accelerate", PhotonTargets.AllBufferedViaServer,1 );
      }
      if(Input.GetMouseButton(1))
      {
        acceleration = -520.5f;
        rb.AddForce(transform.forward * acceleration * Time.deltaTime);
        // this.photonView.RPC( "Accelerate", PhotonTargets.AllBufferedViaServer,1 );
      }
      if(Input.GetKey(KeyCode.W))
      {
        acceleration = -520.5f;
        rb.AddForce(transform.right * acceleration * Time.deltaTime);
        // this.photonView.RPC( "Accelerate", PhotonTargets.AllBufferedViaServer,1 );
      }
      if(Input.GetKey(KeyCode.S))
      {
        acceleration = 520.5f;
        rb.AddForce(transform.right * acceleration * Time.deltaTime);
        // this.photonView.RPC( "Accelerate", PhotonTargets.AllBufferedViaServer,1 );
      }
      if(Input.GetKey(KeyCode.A))
      {

        transform.Rotate(0,-0.5f,0);
        // this.photonView.RPC( "Accelerate", PhotonTargets.AllBufferedViaServer,1 );
      }
      if(Input.GetKey(KeyCode.D))
      {
        transform.Rotate(0,0.5f,0);
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

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            correctPlayerPos = transform.position;
            correctPlayerRot = transform.rotation;
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            //Network player, receive data
            correctPlayerPos = (Vector3)stream.ReceiveNext();
            correctPlayerRot = (Quaternion)stream.ReceiveNext();

        }
    }
}
