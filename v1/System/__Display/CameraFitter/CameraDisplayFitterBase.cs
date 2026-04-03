using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOSG.System.Display
{
    public abstract class CameraDisplayFitterBase
    {
        public abstract float GetAccumulatedCameraSize(float orthoSize, float screenRatio);
    }
}