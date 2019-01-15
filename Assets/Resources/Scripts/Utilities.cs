using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public struct GameData
{
    public Vector3 playerPosition;
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
        string fileOutName = Application.persistentDataPath + Path.DirectorySeparatorChar + "save.dat";
        BinaryWriter binOut;

        if (File.Exists(fileOutName))
        {
            binOut = new BinaryWriter(File.Open(fileOutName, FileMode.Create));
        }
        else
        {
            binOut = new BinaryWriter(File.Create(fileOutName));
        }

        // add data to file
        binOut.Write(dataOut.playerPosition.x);
        binOut.Write(dataOut.playerPosition.y);
        binOut.Write(dataOut.playerPosition.z);

        binOut.Close();
        return true;
    }

    public static GameData LoadGame(string fileName = "save.dat")
    {
        string fileInName = Application.persistentDataPath + Path.DirectorySeparatorChar + fileName;
        BinaryReader binIn;
        GameData dataIn = new GameData();

        if (File.Exists(fileInName))
        {
            binIn = new BinaryReader(File.Open(fileInName, FileMode.Open));
        }
        else
        {
            dataIn.successfulRead = false;
            return dataIn;
        }
        dataIn.playerPosition.x = binIn.ReadSingle();
        dataIn.playerPosition.y = binIn.ReadSingle();
        dataIn.playerPosition.z = binIn.ReadSingle();

        binIn.Close();
        dataIn.successfulRead = true;
        return dataIn;
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
        if (Mathf.Abs(Input.GetAxis("Vertical")) > Controls.deadZone || Mathf.Abs(Input.GetAxis("Horizontal")) > Controls.deadZone)
            return Mathf.Atan2(Input.GetAxis("Vertical"), -1 * Input.GetAxis("Horizontal"));
        return Controls.neutralStickPosition; // return a value well outside of the range of Atan2
    }
    public static float GetMovementStickMagnitude()
    {
        float horizontalValue = Mathf.Abs(Input.GetAxis("Horizontal"));
        if (horizontalValue < Controls.deadZone)
        {
            horizontalValue = 0f;
        }

        float verticalValue = Mathf.Abs(Input.GetAxis("Vertical"));
        if (verticalValue < Controls.deadZone)
        {
            verticalValue = 0f;
        }
        Vector2 magVec = new Vector2(horizontalValue, verticalValue);
        return magVec.magnitude;
    }
    #endregion
}
