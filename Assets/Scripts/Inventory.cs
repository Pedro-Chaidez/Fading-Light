using UnityEngine;
using System.Collections.Generic; 

public class Inventory : MonoBehaviour
{
    private readonly static int LIST_CAPACITY = 5;
    [SerializeField]
    private List<Item> items = new List<Item>(LIST_CAPACITY);
    [SerializeField]
    private int selectedItem = 0;
    public void GetItem(Item newItem)
    {
        if (items[selectedItem] == null)
        {
            items[selectedItem] = newItem;
        }
        else
        {
            Debug.Log("Tried to add an item on a slot with an item already there");
        }
    }
    public void AddItem(Item newItem)
    {
        if (items.Count < LIST_CAPACITY)
        {
            items.Add(newItem);
            Debug.Log("Added " + newItem.name);
        }
        else
        {
            Debug.Log("Inventory is full!");
        }
    }
    public void UseItem()
    {
        if (items[selectedItem] != null)
        {
            Debug.Log("Used " + items[selectedItem].name);
            items.RemoveAt(selectedItem);
        }
        else
        {
            Debug.Log("Tried to use an Item that is not there");
        }
    }
    public void DropItem()
    {
        if (items[selectedItem] != null)
        {
            Debug.Log("Dropped " + items[selectedItem].name);
            items.RemoveAt(selectedItem);
        }
        else
        {
            Debug.Log("Tried to drop an Item that is not there");
        }
    }
    public void scrollUp()
    {
        if (items.Count == 0) return;

        if (selectedItem < items.Count - 1)
        {
            selectedItem++;
        }
        else
        {
            selectedItem = 0;
        }
    }

    public void scrollDown()
    {
        if (items.Count == 0) return;

        if (selectedItem > 0)
        {
            selectedItem--;
        }
        else
        {
            selectedItem = items.Count - 1;
        }
    }
}