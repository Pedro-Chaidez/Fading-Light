using UnityEngine;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using System.Linq;
public class Inventory : MonoBehaviour
{
    private readonly static int LIST_CAPACITY = 5;
    private List<Item> items = new List<Item>(LIST_CAPACITY);
    [SerializeField]
    private int selectedItem = 0;
    private InputManager inputManager;
    private void Start()
    {
        inputManager = GetComponent<InputManager>();
    }
    public Item GetItem()
    {
        return new Battery();
    }
    public void AddItem(Item newItem)
    {
        items.Append(newItem);
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
