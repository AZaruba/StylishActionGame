using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct GameData
{
    public Vector3 playerPosition;
    public Vector3 cameraPosition;
    public bool successfulRead;
}

public static class Utilities {
    // This class contains functions that may be useful in a variety of contexts

    #region Constants
    public const int defInt = int.MinValue;
    public const float defFloat = Mathf.NegativeInfinity;

    // layer masks
    public const int environmentOnly = 1 << 8;
    #endregion

    #region FileIO
    public static bool SaveGame(GameData dataOut)
    {
        return SaveManager.SaveGame(dataOut);
    }

    public static GameData LoadGame(string fileName = "save.dat")
    {
        return SaveManager.LoadGame(fileName);
    }
    #endregion

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
        float Vertical = InputBuffer.moveVertical;
        float Horizontal = InputBuffer.moveHorizontal;

        if (Mathf.Abs(Vertical) > 0 || Mathf.Abs(Horizontal) > 0)
            return Mathf.Atan2(Vertical, -1 * Horizontal);

        return Controls.neutralStickPosition; // return a value well outside of the range of Atan2
    }

    public static float GetCameraStickPosition()
    {
        float camVertical = InputBuffer.cameraVertical;
        float camHorizontal = InputBuffer.cameraHorizontal;

        if (Mathf.Abs(camVertical) > 0 || Mathf.Abs(camHorizontal) > 0)
            return Mathf.Atan2(camVertical, -1 * camHorizontal);

        return Controls.neutralStickPosition; // return a value well outside of the range of Atan2
    }

    public static float GetMovementStickMagnitude()
    {
        float horizontalValue = Mathf.Abs(InputBuffer.moveHorizontal);
        float verticalValue = Mathf.Abs(InputBuffer.moveVertical);

        Vector2 magVec = new Vector2(horizontalValue, verticalValue);
        return magVec.magnitude;
    }

    public static float GetCameraStickMagnitude()
    {
        float horizontalValue = Mathf.Abs(InputBuffer.cameraHorizontal);
        float verticalValue = Mathf.Abs(InputBuffer.cameraVertical);

        Vector2 magVec = new Vector2(horizontalValue, verticalValue);
        return magVec.magnitude;
    }

    /* Added but currently unneeded
    public static Vector3 ConvertStickToVector(float degree)
    {
        Vector3 direction = new Vector3();

        direction.x = -1 * Mathf.Cos(degree) + Mathf.Sin(degree);
        direction.z = Mathf.Sin(degree) + Mathf.Cos(degree);

        return direction;
    }
    */
    #endregion
}
