using System;
using UnityEngine;

public class PlayerCamera : MonoBehaviour, IDisposable
{
    public CameraManager manager;
    public Player player;
    public Camera cam;
    public float x;
    public float y;
    public float width;
    public float height;
    /// <summary>
    /// When set, the camera will temporarily follow this object instead.  Set to null to follow the player again.
    /// </summary>
    public Transform alternativeFollowTarget;
    public bool snapToRoomGrid = true;

    private RectTransformUtility roomSizeReference
    {
        get => manager.gameManager.map.roomSizeReference;
    }
    void Awake() {
        if(cam == null) cam = GetComponent<Camera>();
        // TODO AudioListener?
    }

    void Update() {
        followTarget();
    }
    void followTarget()
    {
        var target = alternativeFollowTarget == null ? player.transform : alternativeFollowTarget;

        float camX = target.position.x;
        float camY = target.position.y;

        if ( snapToRoomGrid )
        {
            // Camera snaps to a grid.
            // A dummy box collider is used as a visual reference of this grid.
            float roomWidth = roomSizeReference.width;
            float roomHeight = roomSizeReference.height;

            var cornerToMiddleOffset = new Vector3( roomWidth, roomHeight ) / 2;

            // Get the target's transform relative to the lower-left corner of the reference box
            var playerRelativeToReferenceLowerLeftCorner =
                target.position - roomSizeReference.transform.position + cornerToMiddleOffset;
            var roomX = Math.Floor( playerRelativeToReferenceLowerLeftCorner.x / roomWidth );
            var roomY = Math.Floor( playerRelativeToReferenceLowerLeftCorner.y / roomHeight );

            camX = (float) (roomSizeReference.transform.position.x + roomX * roomWidth);
            camY = (float) (roomSizeReference.transform.position.y + roomY * roomHeight);
        }

        cam.transform.position = new Vector3( camX, camY, cam.transform.position.z );
    }

    // Call when fields have been changed and the camera will update itself to visually reflect the changes.
    public void onLayoutChange() {
        cam.rect = new Rect(x, y, width, height);
    }

    public void Dispose()
    {
        // TODO remove gameObject?
        // Remove self from manager?  Or is manager responsible for that?
    }
}
