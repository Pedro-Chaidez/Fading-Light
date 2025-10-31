using UnityEngine;

public abstract class Item : Interactable
{
    protected string itemName;
    protected string itemType;
    protected float durability;
    protected abstract void useItem();
}
