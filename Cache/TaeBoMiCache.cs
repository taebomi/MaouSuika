using UnityEngine;

public static class TaeBoMiCache
{
    public static readonly float[] RightRotation = { 90f, 180f, 270f, 0f };

    public static float GetRandomRightRotation()
    {
        return RightRotation[Random.Range(0, RightRotation.Length)];
    }

    public enum TwoDirection
    {
        Left,
        Right
    }


    #region Sorting Layer

    public static readonly int GashaponSortingLayerID = SortingLayer.NameToID("Gashapon");
    public static readonly int ObjectSortingLayerID = SortingLayer.NameToID("Object");
    public static readonly int ForegroundSortingLayerID = SortingLayer.NameToID("Foreground");
    public static readonly int EffectSortingLayerID = SortingLayer.NameToID("Effect");

    #endregion

    #region Layer

    #endregion
}