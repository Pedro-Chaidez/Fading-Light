using UnityEngine;

public abstract class Item : Interactable
{
    public string itemName;
    public Sprite icon;
    protected string itemType;
    protected float durability;
    protected abstract void useItem();
    protected override void Interact()
    {
        // 1. Add this item to the inventory
        Inventory.instance.AddItem(this);

        // 2. Parent it to the Inventory object to keep the scene clean (optional but recommended)
        transform.parent = Inventory.instance.transform;

        // 3. Instead of Destroy(), just deactivate the object.
        // This keeps the reference in the inventory list valid.
        gameObject.SetActive(false);
    }
}
