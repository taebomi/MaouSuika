using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOSG.System.Display
{
    public class TopCameraDisplayFitter : CameraDisplayFitterBase
    {
        public override float GetAccumulatedCameraSize(float orthoSize, float screenRatio)
        {
            if (screenRatio <= DisplayData.MAX_SCREEN_RATIO)
            {
                return orthoSize;
            }

            return orthoSize * screenRatio / DisplayData.MAX_SCREEN_RATIO;
        }
    }
}