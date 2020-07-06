using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using UnityEngine;
using UnityEngine.Assertions;

class CameraSidebarLayout : CameraLayoutEngine
{
    [Description( @"
        Picks one of these based on the number of players.
        Then positions cameras to match the rectangles in this reference.
    " )]
    public List<CameraLayoutReference> layoutReferences = new List<CameraLayoutReference>();

    protected override void OnDisable()
    {
        base.OnDisable();
        // Remove all viewport follower components
        for ( var i = 0; i < manager.cameraCountIncludingDummies; i++ )
        {
            var camera = manager.getCameraAtIncludingDummies( i );
            var component = camera.GetComponent<ViewportFollowRectTransform>();
            if ( component != null ) Destroy( component );
        }
    }

    public override void layout()
    {
        var cameraCount = manager.cameraCountIncludingDummies;

        // Figure out which layout to use based on number of players
        CameraLayoutReference layout = null;
        foreach ( var l in layoutReferences )
        {
            if ( l.maximumPlayers >= cameraCount )
            {
                layout = l;
                break;
            }
        }

        if ( layout == null )
        {
            Debug.Log( "no layout can handle this many players" );
            return;
        }

        var nonLocalPlayerIndex = 0;
        // HACK do not assign more than one camera to the "local player" position since
        // all dummy cameras are also following the local player.
        var assignedLocalPlayerCamera = false;
        for ( var i = 0; i < cameraCount; i++ )
        {
            var camera = manager.getCameraAtIncludingDummies( i );
            var viewportFollow = camera.GetComponent<ViewportFollowRectTransform>();
            // assign a viewport follower component on-demand
            if ( null == viewportFollow )
            {
                viewportFollow = camera.gameObject.AddComponent<ViewportFollowRectTransform>();
                viewportFollow._camera = camera.cam;
            }

            RectTransformUtility t;
            if ( !assignedLocalPlayerCamera && camera.player.photonView.isMine )
            {
                t = layout.getLocalPlayerCamera();
                assignedLocalPlayerCamera = true;
            }
            else
            {
                t = layout.getNonLocalPlayerCamera( nonLocalPlayerIndex++ );
            }

            viewportFollow.target = t.transform as RectTransform;
        }
    }
}
