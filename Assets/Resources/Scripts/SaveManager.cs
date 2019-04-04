using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour {

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
        binOut.Write((int)DataType.PlayerPosition);
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
        DataType type = (DataType)binIn.ReadInt32();
        dataIn.successfulRead = ReadItem(ref binIn, ref dataIn, type);

        binIn.Close();
        return dataIn;
    }

    private static bool ReadItem(ref BinaryReader binIn, ref GameData dataIn, DataType type = DataType.ErrorType)
    {
        switch (type)
        {
            case (DataType.PlayerPosition):
            {
                dataIn.playerPosition.x = binIn.ReadSingle();
                dataIn.playerPosition.y = binIn.ReadSingle();
                dataIn.playerPosition.z = binIn.ReadSingle();
                break;
            }
            case (DataType.CameraPosition):
            {
                break;
            }
            case (DataType.ErrorType):
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    ///  File format will be a data type followed by the data associated with that type.
    ///  This allows different kinds of data to be written in any order and read gracefully.
    /// </summary>
    private enum DataType
    {
        ErrorType = -1,
        PlayerPosition = 0, // may want to replace this with designated save points, this is for testing
        CameraPosition,
        // continue...
    }
}
