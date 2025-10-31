using System.Drawing;
using UnityEngine;
public class SpeedBoost : Item
{
    private float duration;
    private float newSpeed;
    private void Start()
    {
        itemName = "SpeedBoost";
        itemType = "Buff";
        durability = 1;
        duration = 60f;
        newSpeed = 25f;
    }
    protected override void useItem()
    {
        Debug.Log("Used Speed boost with duration: " + duration + "and new speed: "+newSpeed);
    }
    protected override void Interact(Inventory playerInventory)
    {
        playerInventory.GetItem(this);
        Destroy(gameObject);
    }
}