using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
  public GameManager gameManager;
  public HazardManager hazardManager;
  public string buttonToListenFor = "A";
  public bool setInManagerList;
  public int spotInHazardList = -1,costToActivate = 1;
  public bool listenForButtonPress = true,startDisabled,toggleOnOff,stayOn,isOn;
  public bool spin,spring,piston,pistonOut;
  public float actionDistance,actionForce,cooldownLoop, angularSpeedTarget,currentSpinSpeed, timer;
  public GameObject buttonIndicator;
  public Vector3 rotationDirection,startPos;
  public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {

      // transform.position = new Vector3(transform.position.x + startPos.x,transform.position.y + startPos.y,transform.position.z + startPos.z);
      if(rb == null){rb = GetComponent<Rigidbody>();}
      startPos = rb.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(listenForButtonPress == true)
        {
            if(Input.GetButtonDown(buttonToListenFor))
            {
              AttemptToActivate();

            }
        }
        // else{if(GetComponent<Renderer>().isVisible){hazardManager.HazardRequestButton(GetComponent<Hazard>());}}

        if(isOn == true)
        {
          if(spin == true)
          {Spin();}
          if(piston == true)
          {
            if(timer<= 0)
            {Piston(pistonOut);}
              else{timer -= Time.deltaTime;}

          }
        }
    }
    public void StopListening()
    {
      listenForButtonPress = false;
      buttonIndicator = null;
    }
    void OnBecameVisible()
    {

      listenForButtonPress = true;
      hazardManager.HazardRequestButton(GetComponent<Hazard>());
      // if(buttonIndicator != null){buttonIndicator.active = true;}
    }

    void OnBecameInvisible()
    {
        listenForButtonPress = false;
        if(buttonIndicator != null){buttonIndicator.active = false;}
    }

    public void SetSpotInList(int newplace, GameManager newGM)
    {
      gameManager = newGM;
      setInManagerList = true;
      spotInHazardList = newplace;
    }

    public void Spin()
    {
      rb.angularVelocity = rb.transform.up * currentSpinSpeed;
      currentSpinSpeed = Mathf.Lerp(currentSpinSpeed,angularSpeedTarget, Time.deltaTime * actionForce);
      // transform.Rotate(rotationDirection.x * currentSpinSpeed,rotationDirection.y * currentSpinSpeed,rotationDirection.z * currentSpinSpeed);
      // if(rb.angularVelocity.magnitude < angularVelocityTarget)
      // {
      //   // rb.AddTorque(new Vector3(1,1,1) * actionForce * Time.deltaTime);
      //
      // }
    }

    public void Piston(bool moveOut)
    {
      if(moveOut == true)
      {
          rb.velocity = transform.up * actionForce;
          if(Vector3.Distance(startPos, rb.transform.position) > actionDistance)
          {
            timer = cooldownLoop;
            pistonOut = false;
          }
      }
      else
      {
        rb.velocity = transform.up * -actionForce;
        if(Vector3.Distance(startPos, rb.transform.position) <= 0.1f)
        {
          timer = cooldownLoop;
          pistonOut = true;
        }
      }


    }


    public void Activate()
    {


      if(startDisabled == true)
      {    gameObject.active = true;} //neutral on for objects that when activated dissappear
      if(toggleOnOff == true)
      { isOn = !isOn;}

      if(spring == true && rb != null)
      {
        rb.velocity = (transform.up * actionForce);

      }
    }
    public void Deactivate()
    {
      if(startDisabled == true)
      {    gameObject.active = false;} //neutral on for objects that when activated dissappear
      if(toggleOnOff == true)
      {isOn = false;}

    }
    public void AttemptToActivate()
    {
      gameManager.AttemptToActivateHazard(spotInHazardList);
    }

    public void OnMouseDown()
    {
      print("clicked " + spotInHazardList);
      AttemptToActivate();
    }

    public void OnTriggerExit(Collider col)
    {
      currentSpinSpeed = currentSpinSpeed * 0.3f;
    }
}
