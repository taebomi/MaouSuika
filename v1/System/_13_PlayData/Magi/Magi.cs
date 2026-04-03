using System;
using UnityEngine.Serialization;

[ES3Serializable]
[Serializable]
public class Magi
{
    public int value;


    public Magi()
    {
    }

    public static Magi CreateInstance()
    {
        return new Magi(500);
    }

    public Magi(int value)
    {
        this.value = value;
    }

    public static bool operator <(Magi magi, int value) => magi.value < value;
    public static bool operator >(Magi magi, int value) => magi.value > value;
    public static bool operator <=(Magi magi, int value) => magi.value <= value;
    public static bool operator >=(Magi magi, int value) => magi.value >= value;
    public static Magi operator -(Magi magi, int value) => new(magi.value - value);
    public static Magi operator +(Magi magi, int value) => new(magi.value + value);

    public override string ToString()
    {
        return value.ToString();
    }
}