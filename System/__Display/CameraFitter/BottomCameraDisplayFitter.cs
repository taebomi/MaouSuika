using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOSG.System.Display
{
    public class BottomCameraDisplayFitter : CameraDisplayFitterBase
    {
        public override float GetAccumulatedCameraSize(float orthoSize, float _)
        {
            return orthoSize;
        }
    }
}