using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;



public static class StageGashaponArea
{
    public const float XRange = 5f;
    public const float YRange = 7f;

    public static bool GetTouchWorldPos(Camera cam, out Vector3 touchWorldPos)
    {
        foreach (var activeTouch in Touch.activeTouches)
        {
            var worldPos = cam.ScreenToWorldPoint(activeTouch.screenPosition);
            worldPos.z = 0f;
            if (worldPos.x < -XRange || worldPos.x > XRange || worldPos.y > YRange)
            {
                continue;
            }

            touchWorldPos = worldPos;
            return true;
        }

        touchWorldPos = Vector3.zero;
        return false;
    }
}