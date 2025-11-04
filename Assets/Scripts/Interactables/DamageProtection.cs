using UnityEngine;
public class DamageProtection : Item
{
    private float duration; // in seconds
    private void Start()
    {
        itemName = "DamageProtection";
        itemType = "Buff";
        durability = 1;
        duration = 60f;
    }
    protected override void useItem()
    {
        Debug.Log("Used Damage protection with duration: " + duration);
    }
    /*void Update()       // an example on how to use
    {
        timer += Time.deltaTime; // Accumulate time

        if (timer >= duration)
        {
            // Duration reached, perform an action
            Debug.Log("Duration reached!");
            timer = 0.0f; // Reset timer
        }
    }*/
    protected override void Interact(Inventory playerInventory)
    {
        playerInventory.AddItem(this);
        Destroy(gameObject);
    }
}