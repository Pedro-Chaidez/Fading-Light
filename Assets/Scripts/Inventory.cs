using UnityEngine;

public class Inventory : MonoBehaviour
{
    private readonly static int LIST_CAPACITY = 5;
    private Item[] items = new Item[LIST_CAPACITY];
    [SerializeField]
    private int selectedItem = 0;
    public Item GetItem()
    {
        return new Battery();
    }
    public void AddItem(Item newItem)
    {
        Debug.Log("Add Item Triggered");
        //items.Append(newItem);
    }
    public void RemoveItem()
    {
        Debug.Log("Drop Item Triggered");
        //items.RemoveAt(selectedItem
    }
    public void scrollUp()
    {
        if (selectedItem < LIST_CAPACITY)
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
        if (selectedItem > 0)
        {
            selectedItem--;
        }
        else
        {
            selectedItem = LIST_CAPACITY;
        }
    }
}
