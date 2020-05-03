using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour
{

  public GameObject car;
  public GameObject greenShell,redShell;
  public float distanceIfFrontOfCar,distanceAboveOfCar;//where to spawn stuff
  public float redShellForce;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      if(Input.GetKeyDown(KeyCode.Alpha1))
      {UsePower(0);}
      if(Input.GetKeyDown(KeyCode.Alpha2))
      {UsePower(1);}
    }



    public void UsePower(int whichPower)
    {
      Vector3 tempvec = car.transform.position + (car.transform.forward * distanceIfFrontOfCar)+ (car.transform.up * distanceAboveOfCar);
      // Vector3 tempvec = new Vector3(car.transform.position.x + distanceIfFrontOfCar,car.transform.position.y + distanceAboveOfCar,car.transform.position.z);
        switch (whichPower)
        {
            case 0:
            //use the manager script to have a semi-inconsistent direction for the green shell
            GameObject greenclone = Instantiate(greenShell,tempvec,car.transform.rotation);
            greenclone.GetComponent<Rigidbody>().AddForce((transform.position - greenclone.transform.position) * redShellForce,ForceMode.Impulse);
              break;
            case 1:
              GameObject redclone = Instantiate(redShell,tempvec,car.transform.rotation);
              // clone.GetComponent<Rigidbody>().AddForce((car.transform.position - clone.transform.position) * redShellForce,ForceMode.Impulse);
              redclone.GetComponent<Missile>().target = car;
              break;
            case 2:
              break;
            case 3:
              break;
            default:
              break;
        }
    }
}
