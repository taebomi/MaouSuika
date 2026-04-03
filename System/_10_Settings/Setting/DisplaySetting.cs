using System;
using SOSG.System.Display;
using UnityEngine.Serialization;

[Serializable, ES3Serializable]
public class DisplaySetting
{
    public Resolution resolution;
    public FPS fps;

    public DisplaySetting()
    {
        resolution = Resolution.Mid;
        fps = FPS.Mid;
    }

    public DisplaySetting(DisplaySetting displaySetting)
    {
        resolution = displaySetting.resolution;
        fps = displaySetting.fps;
    }

    public bool IsEqual(DisplaySetting displaySetting)
    {
        // ReSharper disable once ReplaceWithSingleAssignment.True
        var isEqual = true;

        if (resolution != displaySetting.resolution)
        {
            isEqual = false;
        }

        if (fps != displaySetting.fps)
        {
            isEqual = false;
        }

        return isEqual;
    }

    public void SetResolutionHeight(Resolution value)
    {
        resolution = value;
    }

    public void SetFPS(FPS value)
    {
        fps = value;
    }

    public enum Resolution
    {
        Low = 0,
        Mid = 1,
        High = 2
    }

    public enum FPS
    {
        Low = 0,
        Mid = 1,
        High = 2
    }
}