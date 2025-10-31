using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public string promptMessage;

    public void ItemInteract(Inventory playerInventory)
    {
        Interact(playerInventory);
    }
    protected virtual void Interact(Inventory playerInventory) { }
}
