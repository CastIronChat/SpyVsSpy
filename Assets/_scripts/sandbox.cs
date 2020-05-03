using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sandbox : MonoBehaviour
{
  public Transform parentObj;
    // Start is called before the first frame update
    void Start()
    {
layoutingrid();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void layoutingrid()
    {
      int count = 0;
      int count1 = 0;
      int count2 = 0;
      foreach(Transform go in parentObj)
      {
        go.transform.localPosition = new Vector3(count1,0,count2);
        count++;
        count1++;

        if(count1 > 9)
        {count1 = 0;count2 ++;}
      }
    }
}
