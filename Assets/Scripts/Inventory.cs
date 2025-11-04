using UnityEngine;
using System.Collections.Generic;
using System;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    private readonly static int LIST_CAPACITY = 5;
    [SerializeField]
    private List<Item> items = new List<Item>(LIST_CAPACITY);
    [SerializeField]
    private int selectedItem = 0;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("More than one instance of inventory found!");
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
            Debug.LogWarning("Tried to use an Item that is not there");
        }
    }
    public void DropItem()
    {
        if(items.Count == 0)
        {
            Debug.Log("Inventory is empty. Cannot drop an item.");
            return;
        }
        try
        {
            if (items[selectedItem] != null)
            {
                Debug.Log("Dropped " + items[selectedItem].name);
                items.RemoveAt(selectedItem);
            }
            else
            {
                Debug.LogWarning("Selected item slot was already empty.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("An unexpected error occurred while dropping an item: " + ex.Message);
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