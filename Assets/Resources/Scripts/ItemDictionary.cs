using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemDictionary {

    private static readonly Dictionary<int, Item> itemLookup =
        new Dictionary<int, Item>
        {
            { 0, new Item("Test Key", "Pickup Test Key") }
        };

    public static Item LookupItem(int id)
    {
        if (!itemLookup.ContainsKey(id))
            return null;

        Item itemOut;
        itemLookup.TryGetValue(id, out itemOut);

        return itemOut;
    }
}
