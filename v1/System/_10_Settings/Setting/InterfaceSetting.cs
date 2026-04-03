using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable, ES3Serializable]
public class InterfaceSetting
{
    public bool suikaMergeGuide;
    public string locale;

    public InterfaceSetting()
    {
        suikaMergeGuide = true;
        locale = "en";
    }

    public InterfaceSetting(InterfaceSetting interfaceSetting)
    {
        suikaMergeGuide = interfaceSetting.suikaMergeGuide;
        locale = interfaceSetting.locale;
    }

    public bool IsEqual(InterfaceSetting interfaceSetting)
    {
        // ReSharper disable once ReplaceWithSingleAssignment.True
        var isEqual = true;

        if (suikaMergeGuide != interfaceSetting.suikaMergeGuide)
        {
            isEqual = false;
        }

        if (locale != interfaceSetting.locale)
        {
            isEqual = false;
        }

        return isEqual;
    }

    public void SetSuikaMergeGuide(bool value)
    {
        suikaMergeGuide = value;
    }

    public void SetLanguage(string newLocaleCode)
    {
        locale = newLocaleCode;
    }
}