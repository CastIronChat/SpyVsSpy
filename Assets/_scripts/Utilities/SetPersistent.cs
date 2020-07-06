using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SetPersistent : MonoBehaviour
{
    [Description(@"
        Calls DontDestroyOnLoad(gameObject)
        so this GameObject persists across scenes.
    ")]

    public bool debugLogging = false;

    void Awake() {
        if(gameObject.scene.IsValid()) {
            if(debugLogging) Debug.Log($"Setting myself as persistent.  Name is {gameObject.name}", this);
            DontDestroyOnLoad(gameObject);
        } else {
            if(debugLogging) Debug.Log($"Not part of a scene; not setting self as persistent.  Name is {gameObject.name}; scene name is {gameObject.scene.name}", this);
        }
    }

}
