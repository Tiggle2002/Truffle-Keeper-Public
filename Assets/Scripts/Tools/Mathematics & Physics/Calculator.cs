using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Calculator
{
  public static bool InRange(Vector3Int posA, Vector3Int posB, int range)
    {
        Vector3Int distanceFromBlock = posA - posB;

        if (Mathf.Abs(distanceFromBlock.x) <= range && Mathf.Abs(distanceFromBlock.y) <= range)
        {
            return true;
        }

        return false;
    }

    public static float PercentageToDecimal(this int percentage)
    {
        return percentage * 0.01f;
    }
}
