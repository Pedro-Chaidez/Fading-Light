using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
public class Inventory : MonoBehaviour
{
    private readonly static int LIST_CAPACITY = 5;
    private List<Item> items = new List<Item>(LIST_CAPACITY);
    public int itemsSize = 0;
    public Item GetItem()
    {
        return new Item();
    }
    public void AddItem(Item newItem)
    {

    }
    public void RemoveItem()
    {

    }
}
