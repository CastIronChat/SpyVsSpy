using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLayoutEngine : MonoBehaviour
{
    public CameraManager manager;
    protected virtual void OnEnable() {
        manager.OnLayoutShouldChange += layout;
    }
    protected virtual void OnDisable() {
        manager.OnLayoutShouldChange -= layout;
    }
    public virtual void OnDestroy() {
        manager.OnLayoutShouldChange -= layout;
    }

    public virtual void layout()
    {
    }
}
