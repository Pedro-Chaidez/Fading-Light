using UnityEngine;
using Unity.Netcode;

public class Respawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private PrefabSpawner playerSpawner;
    void Start()
    {
        // Check if NetworkManager is active (multiplayer mode)
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            // Don't spawn in multiplayer - NetworkManager handles player spawning
            Debug.Log("[Respawner] NetworkManager is active - skipping spawn. NetworkManager handles player spawning.");
            return;
        }

        // Single player mode - spawn normally
        playerSpawner = GetComponent<PrefabSpawner>();
        if (playerSpawner != null)
        {
            playerSpawner.RespawnPrefab();
        }
    }
}
