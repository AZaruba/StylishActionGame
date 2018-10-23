using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {

    private string itemName;
    private string itemDescr;
    private int itemId;

	public Item(string itemName)
    {
        this.itemName = itemName;
        this.itemDescr = "";
        this.itemId = -1;
    }

    public Item(string itemName, string itemDescr)
    {
        this.itemName = itemName;
        this.itemDescr = itemDescr;
        this.itemId = -1;
    }

    public Item(string itemName, string itemDescr, int id)
    {
        this.itemName = itemName;
        this.itemDescr = itemDescr;
        this.itemId = id;
    }

    public string getName()
    {
        return itemName;
    }
    
    public string getDescr()
    {
        return itemDescr;
    }
}
