using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    private List<Item> heldItems;
	// Use this for initialization
	void Start () {
        heldItems = new List<Item>();
	}

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(Controls.Interact))
        {
            RaycastHit doorCheck;
            if (Physics.SphereCast(transform.position, 2, Vector3.zero, out doorCheck, 1 << 13))
            {
                Door getDoor = doorCheck.collider.gameObject.GetComponent<Door>();
                if (getDoor == null)
                    return;

                bool foundKey = false;
                for (int x = 0; x < heldItems.Count; x++)
                {
                    foundKey = getDoor.OpenDoor(heldItems[x].GetId());
                    if (foundKey)
                    {
                        getDoor.OpenAnimate();
                        return;
                    }
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer != 12)
        {
            return;
        }

        if (Input.GetKey(KeyCode.JoystickButton1))
        {
            ItemTrigger it = collision.gameObject.GetComponent<ItemTrigger>();
            if (PickUpItem(it))
                Destroy(collision.gameObject);
        }

    }

    private bool PickUpItem(ItemTrigger it)
    {
        if (!it)
        {
            return false;
        }

        Item newItem = ItemDictionary.LookupItem(it.GetItemId());

        if (newItem == null)
        {
            return false;
        }

        heldItems.Add(newItem);
        Debug.Log(heldItems[0].getName() + ": " + heldItems[0].getDescr());
        return true;
    }
}
