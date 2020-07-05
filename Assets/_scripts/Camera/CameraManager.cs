using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CameraManager : MonoBehaviour
{
    public PlayerManager playerManager;

    public GameObject cameraPrefab;

    public RectTransformUtility roomSizeReference;
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
        // TODO this runs on the prefab which, by necessity, has nulls here.
        // So we must skip these assertions to avoid annoying log messages.

        // Assert.IsNotNull(playerManager);
        Assert.IsNotNull(cameraPrefab);
        // Assert.IsNotNull(roomSizeReference);
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
        Assert.IsNotNull(p);
        var gameObject = UnityEngine.Object.Instantiate(cameraPrefab);
        var camera = gameObject.GetComponent<PlayerCamera>();
        camera.player = p;
        camera.manager = this;
        return camera;
    }
}
