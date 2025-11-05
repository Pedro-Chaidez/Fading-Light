using UnityEngine;

public class Button : Interactable
{
    [SerializeField]
    private PrefabSpawner spawnLocation;
    protected override void Interact()
    {
        spawnLocation.RespawnPrefab();
        Debug.Log("Calling RespawnPrefab");
    }
}
