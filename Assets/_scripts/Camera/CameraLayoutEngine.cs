using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CameraLayoutEngine : MonoBehaviour
{
    public CameraManager manager;
    void Awake() {
        if(this.enabled) {
            manager.OnLayoutShouldChange += layout;
        }
    }
    void Start() {}
    void OnDestroy() {
        manager.OnLayoutShouldChange -= layout;
    }
    public abstract void layout();
}
