using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item {

    private string itemName;
    private string itemDescr;

	public Item(string itemName)
    {
        this.itemName = itemName;
        this.itemDescr = "";
    }

    public Item(string itemName, string itemDescr)
    {
        this.itemName = itemName;
        this.itemDescr = itemDescr;
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
