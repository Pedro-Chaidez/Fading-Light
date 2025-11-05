using UnityEngine;

public class Button : Interactable
{
    [SerializeField]
    private ItemSpawnLocation spawnLocation;
    protected override void Interact()
    {
        spawnLocation.RespawnPrefab();
        Debug.Log("Calling RespawnPrefab");
    }
}
