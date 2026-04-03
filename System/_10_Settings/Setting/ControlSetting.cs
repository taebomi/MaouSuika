using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable, ES3Serializable]
public class ControlSetting
{
    public bool invertYAxis;
    public float dragRange;

    public const float MinDragRange = 1.5f;
    public const float MaxDragRange = 5f;

    public ControlSetting()
    {
        invertYAxis = false;
        dragRange = 3f;
    }

    public ControlSetting(ControlSetting controlSetting)
    {
        invertYAxis = controlSetting.invertYAxis;
        dragRange = controlSetting.dragRange;
    }

    public bool IsEqual(ControlSetting controlSetting)
    {
        // ReSharper disable once ReplaceWithSingleAssignment.True
        var isEqual = true;

        if (invertYAxis != controlSetting.invertYAxis)
        {
            isEqual = false;
        }

        if (Math.Abs(dragRange - controlSetting.dragRange) > 0.1f)
        {
            isEqual = false;
        }

        return isEqual;
    }

    public void SetDragInverse(bool value)
    {
        invertYAxis = value;
    }

    public void SetDragRange(float value)
    {
        dragRange = Mathf.Lerp(MinDragRange, MaxDragRange, value);
    }
}