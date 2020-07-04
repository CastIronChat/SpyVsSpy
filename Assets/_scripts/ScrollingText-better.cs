using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingTextBetter : MonoBehaviour
{
    public Transform scrollingTextParent;
    public float timer;
    public float timeToClearMessage = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        if(scrollingTextParent == null) scrollingTextParent = transform;
        StartCoroutine(ScrollOldLines());
    }

    public IEnumerator ScrollOldLines() {
        while(true) {
            do {
                timer -= Time.deltaTime;
                yield return null;
            } while(timer > 0);
            NewLine("");
        }
    }

    public void NewLine(string newline)
    {
        // Shift all lines of text downward
        int count = LineCount - 1;
        while ( count > 0 )
        {
            getRow(count).text = getRow(count - 1).text;
            count--;
        }
        // Set line 0 to new text
        var row = getRow(0);
        row.text = newline;

        timer = timeToClearMessage;
    }
    private int LineCount {
        get => scrollingTextParent.childCount;
    }
    private ScrollingTextRow getRow(int index) {
        return new ScrollingTextRow(scrollingTextParent.GetChild(index));
    }
}

public class ScrollingTextRow {
    public ScrollingTextRow(Transform row) {
        this.row = row;
    }
    private Transform row;
    public string text {
        get => textComponent.text;
        set {
            textComponent.text = value;
            background.active = value.Length > 0;
        }
    }
    public Text textComponent {
        get => row.GetChild( 1 ).GetComponent<Text>();
    }
    public GameObject background {
        get => row.GetChild(0).gameObject;
    }
}
