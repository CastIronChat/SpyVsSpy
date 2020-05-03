using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
  public GameObject target;
  public float speed = 5.0f,rotspeed = 1.0f,bounceCoolDownTime = 1.0f,bounceTimer;
  private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
      if(target != null && bounceTimer <= 0)
      {
        TurnToFace();
        rb.AddForce(transform.forward * speed * Time.deltaTime);
      }
    }

    public void TurnToFace()
    {

        transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (target.transform.position - transform.position), rotspeed * Time.deltaTime);

    }
    public void OnCollisionEnter(Collision col)
    {
      if(col.gameObject == target)
      {
        col.gameObject.GetComponent<Rigidbody>().AddForce(((transform.forward * 2) + transform.up).normalized * rb.velocity.magnitude * 3,ForceMode.Impulse);
        // bounceTimer = bounceCoolDownTime;
      }
    }
}
