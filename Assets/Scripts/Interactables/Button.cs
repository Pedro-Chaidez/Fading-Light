using UnityEngine;

public class Button : Interactable
{
    protected override void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
    }
}
