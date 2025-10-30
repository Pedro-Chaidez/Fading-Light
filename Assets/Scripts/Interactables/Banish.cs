using UnityEngine;
public class Banish : Item
{
    private string color;
    private void Start()
    {
        itemName = "Banish";
        itemType = "Consumable";
        durability = 1;
        color = "grey";
    }
    protected override void Interact()
    {
        Debug.Log("Interacted with Banish and it has the color: "+color);
    }
}