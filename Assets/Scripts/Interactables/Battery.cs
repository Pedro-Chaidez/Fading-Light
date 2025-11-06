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
}
