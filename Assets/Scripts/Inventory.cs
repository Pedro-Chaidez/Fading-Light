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
    [SerializeField]
    private GameObject[] slotIcons;

    [SerializeField]
    private Transform dropPoint;

    public void Start() {
        UpdateUI();
    }
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
            Debug.Log("Added " + newItem.itemName);
            UpdateUI();
        }
        else
        {
            Debug.Log("Inventory is full!");
        }
    }

    private void UpdateUI() {
        for (int i = 0; i < slotIcons.Length; i++) {
            slotIcons[i].SetActive(false);
        }
        
        for (int i = 0; i < items.Count; i++) {
            slotIcons[i].SetActive(true);
        }
    }
    public void UseItem()
    {
        if(items.Count == 0)
        {
            Debug.LogWarning("Inventory is empty!");
        }
        else if (items[selectedItem] != null  && items[selectedItem].itemName != "Banish")
        {
            Debug.Log("Used " + items[selectedItem].itemName);
            Destroy(items[selectedItem].gameObject);
            items.RemoveAt(selectedItem);
        }
        else
        {
            Debug.LogWarning("Tried to use an Item that is not there");
        }
    }

    public void UseItem_Banish()
    {
        if (items.Count == 0)
        {
            Debug.LogWarning("Inventory is empty!");
        }
        else if (items[selectedItem] != null && items[selectedItem].itemName == "Banish")
        {
            Debug.Log("Used " + items[selectedItem].itemName);
            Destroy(items[selectedItem].gameObject);
            items.RemoveAt(selectedItem);
            UpdateUI();
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
                Item itemToDrop = items[selectedItem];

                // Use .itemName
                Debug.Log("Dropped " + itemToDrop.itemName);

                // --- THIS IS THE FIX ---
                // 1. Un-parent the item
                itemToDrop.transform.parent = null;

                // 2. Set its position to the drop point (or in front of the player)
                if (dropPoint != null)
                {
                    itemToDrop.transform.position = dropPoint.position;
                }
                else
                {
                    // Fallback if no dropPoint is assigned
                    itemToDrop.transform.position = transform.position + (transform.forward * 2);
                }

                // 3. Reactivate it so it appears in the world
                itemToDrop.gameObject.SetActive(true);

                // 4. Remove it from the inventory list
                items.RemoveAt(selectedItem);
                UpdateUI();
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

    public bool isBanish()
    {
        if (items[selectedItem] != null && items[selectedItem].itemName == "Banish")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}