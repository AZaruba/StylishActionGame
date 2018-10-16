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
        if (Input.GetKeyDown(KeyCode.Joystick1Button1))
        {
            Debug.Log("Inventory size: " + heldItems.Count);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != 12)
            return;

        ItemTrigger it = collision.gameObject.GetComponent<ItemTrigger>();

        if (!it)
            return;

        Item newItem = ItemDictionary.LookupItem(it.GetItemId());

        if (newItem == null)
            return;

        heldItems.Add(newItem);
        Destroy(collision.gameObject);

        Debug.Log(heldItems[0].getName() + ": " + heldItems[0].getDescr());
    }
}
