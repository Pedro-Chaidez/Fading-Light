using UnityEngine;

public abstract class Item : Interactable
{
    protected string itemName;
    protected string itemType;
    protected float durability;
    protected virtual void CopyFrom(Item item)
    {
        this.name = item.name;
        this.itemType = item.itemType;
        this.durability = item.durability;
    }
}
