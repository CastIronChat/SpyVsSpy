using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
  public GameManager gameManager;
  public bool setInManagerList;
  public int spotInHazardList = -1;
  public bool startDisabled,toggleOnOff,stayOn,isOn;
  public bool spin,spring;
  public float actionForce,angularSpeedTarget,currentSpinSpeed;
  public Vector3 rotationDirection,startPos;
  private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
      transform.position = new Vector3(transform.position.x + startPos.x,transform.position.y + startPos.y,transform.position.z + startPos.z);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
      if(isOn == true)
      {
        if(spin == true)
        {Spin();}

      }
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


    public void Activate()
    {


      if(startDisabled == true)
      {    gameObject.active = true;} //neutral on for objects that when activated dissappear
      if(toggleOnOff == true)
      { isOn = !isOn;}

      if(spring == true && rb != null)
      {rb.velocity = (transform.up * actionForce);}
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
