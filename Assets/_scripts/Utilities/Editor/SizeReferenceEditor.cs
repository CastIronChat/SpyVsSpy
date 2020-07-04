using UnityEditor;
using UnityEngine;

// [CustomEditor(typeof(SizeReference))]
public class SizeReferenceEditor : Editor {
    void OnSceneGUI() {
        Handles.DrawCamera(new Rect(0,0,500,500), Camera.current);
        // Handles.PositionHandle(Vector3.zero, Quaternion.identity);
    }
}
