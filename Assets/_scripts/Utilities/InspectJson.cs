using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class InspectJson : MonoBehaviour
{
    [Description(@"
        Shows a target object serialized as JSON.
    ")]
    [TextArea(3, 30)]
    public string a;
    public bool on = false;
    public UnityEngine.Object target;
    void Update() {
        if(on && target != null) {
            a = JsonUtility.ToJson(target, true);
        } else {
            a = "";
        }
    }
}
