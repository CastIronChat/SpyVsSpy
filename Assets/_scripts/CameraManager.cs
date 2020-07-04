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

    List<PlayerCamera> cameras = new List<PlayerCamera>();
    List<PlayerCamera> dummyCameras = new List<PlayerCamera>();
    Dictionary<Player, PlayerCamera> playerToCamera = new Dictionary<Player, PlayerCamera>();

    // For now, all these numbers are in screen units 0 to 1.
    // TODO ask Dan if there's a better way to think about camera viewport values relative to Canvas dimensions.
    public float borderMargin = 0;
    public float betweenCamerasMargin = 0;
    // TODO margins should probably shrink to be some % of size of cameras


    /// Set true in inspector to play with the layout values
    public bool forceReLayoutEveryFrame = false;

    /// Set in inspector to force the creation of extras cameras, for debugging.
    [Range(0, 18)]
    public int addDummyCameras = 0;

    void OnValidate() {
        // TODO this runs on the prefab which, by necessity, has nulls here.
        // So we must skip these assertions to avoid annoying log messages.

        // Assert.NotNull(playerManager);
        Assert.NotNull(cameraPrefab);
        // Assert.NotNull(roomSizeReference);
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
            if(cameras.Count > 0) {
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

                // TODO re-sort the camera list

                // Determine layout: longest row and # of rows
                int cameraCount = cameras.Count + dummyCameras.Count;
                int maxRowWidth = 1;
                if(cameraCount > 1) maxRowWidth = 2;
                if(cameraCount > 4) maxRowWidth = 3;
                if(cameraCount > 9) maxRowWidth = 4;
                if(cameraCount > 16) maxRowWidth = 5;
                int rowCount = cameraCount / maxRowWidth + (cameraCount % maxRowWidth > 0 ? 1 : 0);
                float widthEatenByMargins = (maxRowWidth - 1) * betweenCamerasMargin + 2 * borderMargin;
                float cameraWidth = (1 - widthEatenByMargins) / maxRowWidth;
                float heightEatenByMargins = (rowCount - 1) * betweenCamerasMargin + 2 * borderMargin;
                float cameraHeight = (1 - heightEatenByMargins) / rowCount;

                // Compute dimensions of a single camera
                // Compute dimensions of camera cluster
                // Compute margins
                int row = 0;
                int col = 0;
                for(var i = 0; i < cameraCount; i++) {
                    var camera = i < cameras.Count ? cameras[i] : dummyCameras[i - cameras.Count];
                    camera.x = borderMargin + col * (cameraWidth + betweenCamerasMargin);
                    camera.y = 1 - borderMargin - row * betweenCamerasMargin - cameraHeight * (row + 1);
                    camera.width = cameraWidth;
                    camera.height = cameraHeight;
                    camera.onLayoutChange();
                    col++;
                    if(col >= maxRowWidth) {
                        col= 0; row++;
                    }
                }
            }
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
