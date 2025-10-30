using UnityEngine;

public class Battery : Item
{
    private void Start()
    {
        itemName = "Battery";
        itemType = "Consumable";
        durability = 100;
    }
    protected override void Interact()
    {
        Debug.Log("Interacted with Battery");
    }
}
