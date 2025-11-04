using UnityEngine;

public class Battery : Item
{
    private void Awake()
    {
        itemName = "Battery";
        itemType = "Consumable";
        durability = 100;
    }
    protected override void useItem()
    {

    }
    protected override void Interact(Inventory playerInventory)
    {
        playerInventory.AddItem(this);
        //Destroy(gameObject); //this stops generation and deletes item, bug adds empty item
    }
}
