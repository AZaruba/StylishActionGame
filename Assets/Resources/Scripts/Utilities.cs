using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {
    // This class contains functions that may be useful in a variety of contexts

    /// <summary>
    /// Checks if two floats are equal
    /// </summary>
    /// <param name="f1"></param>
    /// <param name="f2"></param>
    /// <param name="nPrecision"></param>
    /// <returns>true if equal, false otherwise</returns>
	public static bool CompareFloats(float f1, float f2, int nPrecision = 6)
    {
        float difference = Mathf.Abs(f1 - f2);
        if (difference < 1 / (Mathf.Pow(10,nPrecision*-1))) // allows an int to be passed instead of long strings of zeroes
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if an int is valid. This software is designed with the assumption
    /// that int.MinValue should not be encountered during normal circumstances
    /// </summary>
    /// <param name="i"></param>
    /// <returns>returns true if integer exists (is not int.MinValue)</returns>
    public static bool CheckInt(int i)
    {
        if (i > int.MinValue)
        {
            return true;
        }
        return false;
    }

    public static int defInt = int.MinValue;
    public static float defFloat = Mathf.NegativeInfinity;
}
