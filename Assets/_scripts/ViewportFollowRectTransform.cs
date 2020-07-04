using UnityEngine;

public class ViewportFollowRectTransform : MonoBehaviour
{
    [Description(@"
        Update a camera viewport to follow the bounding box of a RectTransform on a Canvas.

        For example, Unity's canvas layout anchors can be used to position the viewport within
        UI elements, and the camera will automatically update accordingly.
    ")]

    public RectTransform target;
    public Camera _camera;
    public Canvas canvas;
    void LateUpdate() {
        _camera.pixelRect = target.ToScreenSpace();
    }
}
