using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool IsEqual(this FingerPosition[] fingerPosition, FingerPosition[] fingerPosition2)
    {
        for (int i = 0; i < fingerPosition.Length; i++)
            if (fingerPosition[i] != fingerPosition2[i])
                return false;
        return true;
    }
}
