using System;

class CameraGridLayout : CameraLayoutEngine {
    public float cameraAspectRatio = 1;

    // TODO ask Dan if there's a better way to think about camera viewport values relative to Canvas dimensions.
    // measured as percentage of screen height
    public float borderMargin = 0;
    // Margin between cameras expressed as a percentage of the smallest camera's height.
    public float betweenCamerasMarginPercentage = 0;
    // TODO margins should probably shrink to be some % of size of cameras

    public override void layout()
    {
            if(manager.cameras.Count > 0) {
                // TODO re-sort the camera list

                // Determine layout: longest row and # of rows
                int cameraCount = manager.cameras.Count + manager.dummyCameras.Count;
                int maxRowWidth = 1;
                if(cameraCount > 1) maxRowWidth = 2;
                if(cameraCount > 4) maxRowWidth = 3;
                if(cameraCount > 9) maxRowWidth = 4;
                if(cameraCount > 16) maxRowWidth = 5;
                int rowCount = cameraCount / maxRowWidth + (cameraCount % maxRowWidth > 0 ? 1 : 0);

                // "Units" refers to a virtual unit where 1 === the height of a viewport camera.
                // Layout size is determined in "Units", then we figure out how small a single "Unit" must be to fit the layout into the screen.

                float cameraHeightInUnits = 1;
                float cameraWidthInUnits = cameraAspectRatio;

                float layoutWidthInUnits = (maxRowWidth * cameraWidthInUnits) + (maxRowWidth - 1) * betweenCamerasMarginPercentage;
                float layoutHeightInUnits = (rowCount * cameraHeightInUnits) + (rowCount - 1) * betweenCamerasMarginPercentage;

                float screenAspectRatio = (float)1280/(float)720;
                float screenHeight = 1;
                float screenWidth = screenAspectRatio;

                float widthAvailableForCameras = screenWidth - 2 * borderMargin;
                float heightAvailableForCameras = screenHeight - 2 * borderMargin;

                float scaleToFitHorizontally = layoutWidthInUnits / widthAvailableForCameras;
                float scaleToFitVertically = layoutHeightInUnits / heightAvailableForCameras;
                // factor of "2" means divide by 2 to scale units to Unity's viewport rect
                float scaleFactor = Math.Max(scaleToFitHorizontally, scaleToFitVertically);

                float hCenteringOffset = 0;
                float vCenteringOffset = 0;
                if(scaleToFitHorizontally > scaleToFitVertically) {
                    vCenteringOffset = (heightAvailableForCameras - (layoutHeightInUnits / scaleFactor)) / 2;
                } else {
                    hCenteringOffset = (widthAvailableForCameras - (layoutWidthInUnits / scaleFactor)) / 2;
                }

                // Enforce camera aspect ratio
                float cameraWidth = cameraWidthInUnits / scaleFactor;
                float cameraHeight = cameraHeightInUnits / scaleFactor;
                float betweenCamerasMargin = betweenCamerasMarginPercentage / scaleFactor;

                // Compute dimensions of a single camera
                // Compute dimensions of camera cluster
                // Compute margins
                int row = 0;
                int col = 0;
                for(var i = 0; i < cameraCount; i++) {
                    var camera = i < manager.cameras.Count ? manager.cameras[i] : manager.dummyCameras[i - manager.cameras.Count];
                    // horizontal values must be crunched more than vertical to account for aspect ratio.  The math done above assumed the screen is wider than 1 unit, but Unity's camera viewport uses 0 to 1.
                    camera.x = (hCenteringOffset + borderMargin + col * (cameraWidth + betweenCamerasMargin)) / screenAspectRatio;
                    camera.y = vCenteringOffset + 1 - borderMargin - row * betweenCamerasMargin - cameraHeight * (row + 1);
                    camera.width = cameraWidth / screenAspectRatio;
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
