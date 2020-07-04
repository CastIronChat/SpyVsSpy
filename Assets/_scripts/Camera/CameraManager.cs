using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    /*
     * Camera grid has different # of rows & cols based on # of cameras:
     * 1 camera - whole screen
     * 2-4 - 2 per row
     * 4 - 2x2 grid
     * 5 - 3 top, 2 below
     * Up to 9 - 3 per row
     * 10-16 - 4 per row
     * 17-21 - 5 per row
     */

    public PlayerManager playerManager;

    public GameObject cameraPrefab;

    public SizeReference roomSizeReference;
    [HideInInspector]
    public List<PlayerCamera> cameras = new List<PlayerCamera>();
    [HideInInspector]
    public List<PlayerCamera> dummyCameras = new List<PlayerCamera>();
    Dictionary<Player, PlayerCamera> playerToCamera = new Dictionary<Player, PlayerCamera>();

    /// Set true in inspector to play with the layout values
    public bool forceReLayoutEveryFrame = false;

    /// Set in inspector to force the creation of extras cameras, for debugging.
    [Range(0, 18)]
    public int addDummyCameras = 0;

    public delegate void OnLayoutShouldChangeDelegate();
    public event OnLayoutShouldChangeDelegate OnLayoutShouldChange;

    void OnValidate() {
        Assert.NotNull(playerManager);
        Assert.NotNull(cameraPrefab);
        Assert.NotNull(roomSizeReference);
    }
    void Update()
    {
        recomputeLayoutIfNeeded();
    }
    void recomputeLayoutIfNeeded()
    {
        // Check if anything has changed: any players added or removed
        // If so, recompute layout

        var activePlayers = playerManager.activePlayers;
        var playersWithCameras = playerToCamera.Keys;
        var dirty = forceReLayoutEveryFrame;
        Player firstPlayer = null;
        foreach ( var p in playersWithCameras )
        {
            if ( !activePlayers.Contains( p ) ) {
                dirty = true;
                var camera = playerToCamera[p];
                playerToCamera.Remove(p);
                cameras.Remove(camera);
                Destroy(camera.gameObject);
            }
        }
        foreach ( var p in activePlayers )
        {
            if(firstPlayer == null) firstPlayer = p;
            if ( !playerToCamera.ContainsKey( p ) ) {
                dirty = true;
                var camera = createCamera(p);
                playerToCamera.Add(p, camera);
                cameras.Add(camera);
            }
        }
        if ( dirty )
        {
            // Remove dummy cameras
            while(dummyCameras.Count > addDummyCameras) {
                var camera = dummyCameras[dummyCameras.Count - 1];
                dummyCameras.Remove(camera);
                Destroy(camera.gameObject);
            }
            // Add dummy cameras
            while(dummyCameras.Count < addDummyCameras) {
                dummyCameras.Add(createCamera(firstPlayer));
            }

            OnLayoutShouldChange();
        }
    }

    private PlayerCamera createCamera(Player p) {
        Assert.NotNull(p);
        var gameObject = UnityEngine.Object.Instantiate(cameraPrefab);
        var camera = gameObject.GetComponent<PlayerCamera>();
        camera.player = p;
        camera.manager = this;
        return camera;
    }
}

/*
 * Desired camera width is a percentage of smallest camera size

 KNOWN:
 Fixed aspect ratio for each viewport
 Spacing is a percentage of viewport *height* (can be converted to width via fixed aspect ratio)

//  Compute max height if screen was infinitely wide
//  Compute max width if screen was infinitely high

 Compute layout height in camera height units.
 Compute layout width in camera *height* units.
 Figure out scaling factor to scrunch the layout into the screen's aspect ratio.
  - Math.Min of Xscale and Yscale

 Compute camera height & width based on scaling factor.

 */
