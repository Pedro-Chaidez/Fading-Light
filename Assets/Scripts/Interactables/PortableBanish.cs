using UnityEngine;
public class PortableBanish : Item
{
    private void Start()
    {
        itemName = "PortableBanish";
        itemType = "Consumable";
        durability = 1;
    }
    protected override void Interact()
    {
        Debug.Log("Interacted with PortableBanish");
    }
}