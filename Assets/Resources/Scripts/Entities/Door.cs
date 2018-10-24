using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

    public int keyId;
    public string descr;

    public int GetKeyId()
    {
        return keyId;
    }

	public bool OpenDoor(int itemId)
    {
        if (itemId == keyId)
        {
            // OpenAnimate();
            return true;
        }
        return false;
    }

    public void OpenAnimate()
    {
        Destroy(gameObject); // currently just checking to see if door interactions work
    }
}
