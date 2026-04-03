using System;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCache : MonoBehaviour
{
    
    private static readonly Dictionary<LayerName, int> NameToLayerDict = new();
    private static readonly Dictionary<LayerName, LayerMask> LayerMaskDict = new();

    public static readonly int SuikaLayerMask = LayerMask.GetMask("Suika");
        
    public static int GetNameToLayer(LayerName layerName)
    {
        if (!NameToLayerDict.TryGetValue(layerName, out var layerInt))
        {
            NameToLayerDict.Add(layerName,
                layerInt = LayerMask.NameToLayer(Enum.GetName(typeof(LayerName), layerName)));
        }

        return layerInt;
    }

    public static LayerMask GetLayerMask(LayerName layerName)
    {
        if (!LayerMaskDict.TryGetValue(layerName, out var layerMask))
        {
            LayerMaskDict.Add(layerName,
                layerMask = LayerMask.GetMask(Enum.GetName(typeof(LayerName), layerName)));
        }

        return layerMask;
    }
    [Flags]
    public enum LayerName
    {
        Gashapon,
        Suika,
    }
}
