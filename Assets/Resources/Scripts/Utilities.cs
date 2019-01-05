using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {
    // This class contains functions that may be useful in a variety of contexts

    #region MathUtilities
    // These functions extend the math/logic functionality of the code

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
    #endregion

    #region InputUtilities
    // These functions interpret raw input for various gameplay purposes

    /// <summary>
    /// Takes the position of the stick and turns it into an angle usable by the game world.
    /// Takes the specified dead zone into account so imprecise joysticks can cooperate.
    /// </summary>
    /// <returns>Returns the position of the stick as an angle between -180 and 180 degrees or a neutral position value</returns>
    public static float GetMovementStickPosition()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) > Controls.deadZone || Mathf.Abs(Input.GetAxis("Horizontal")) > Controls.deadZone)
            return Mathf.Atan2(Input.GetAxis("Vertical"), -1 * Input.GetAxis("Horizontal"));
        return Controls.neutralStickPosition; // return a value well outside of the range of Atan2
    }
    #endregion

    #region Constants
    public const int defInt = int.MinValue;
    public const float defFloat = Mathf.NegativeInfinity;

    // layer masks
    public const int environmentOnly = 1 << 8;
    #endregion
}
