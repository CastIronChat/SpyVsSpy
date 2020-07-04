using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingText : MonoBehaviour
{
    public Transform scrollingTextParent;
    public float timer, timeToClearMessage = 10.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if ( timer > 0 )
        {
            timer -= Time.deltaTime;
            if ( timer <= 0 ) { NewLine( "" ); }
        }

    }

    public void NewLine(string newline)
    {
        int count = scrollingTextParent.childCount - 1;
        while ( count > 0 )
        {
            scrollingTextParent.GetChild( count ).GetChild( 1 ).GetComponent<Text>().text = scrollingTextParent.GetChild( count - 1 ).GetChild( 1 ).GetComponent<Text>().text;
            if ( scrollingTextParent.GetChild( count ).GetChild( 1 ).GetComponent<Text>().text.Length > 0 )
            { scrollingTextParent.GetChild( count ).GetChild( 0 ).gameObject.SetActive( true ); }
            else { scrollingTextParent.GetChild( count ).GetChild( 0 ).gameObject.SetActive( false ); }
            count--;
        }
        scrollingTextParent.GetChild( 0 ).GetChild( 1 ).GetComponent<Text>().text = newline;
        if ( scrollingTextParent.GetChild( 0 ).GetChild( 1 ).GetComponent<Text>().text.Length > 0 )
        { scrollingTextParent.GetChild( 0 ).GetChild( 0 ).gameObject.SetActive( true ); }
        else { scrollingTextParent.GetChild( 0 ).GetChild( 0 ).gameObject.SetActive( false ); }

        timer = timeToClearMessage;
    }
}
