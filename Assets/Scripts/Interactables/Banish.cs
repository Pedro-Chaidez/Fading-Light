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
    protected override void useItem()
    {
        Debug.Log("Used Banish with color: " + color);
    }
}