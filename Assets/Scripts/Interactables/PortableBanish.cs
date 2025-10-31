using UnityEngine;
public class PortableBanish : Item
{
    private string color;
    private void Start()
    {
        itemName = "PortableBanish";
        itemType = "Consumable";
        durability = 1;
        color = "rainbow";
    }
    protected override void useItem()
    {
        Debug.Log("Used Portable banish with color: " + color);
    }
    protected override void Interact(Inventory playerInventory)
    {
        playerInventory.GetItem(this);
        Destroy(gameObject);
    }
}