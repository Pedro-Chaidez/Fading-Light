using UnityEngine;
using Unity.Netcode;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabToSpawn; 
    private Transform respawnPoint;
    [SerializeField]
    private bool spawnOnStart;

    private void Awake()
    {
        // Check if NetworkManager is active (multiplayer mode) - check early in Awake
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            // Disable this script in multiplayer - NetworkManager handles spawning
            Debug.Log($"[PrefabSpawner] NetworkManager is active - disabling PrefabSpawner on {gameObject.name}. Use NetworkPrefabSpawner for multiplayer.");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        // Double-check in Start (in case NetworkManager starts between Awake and Start)
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            // Disable this script in multiplayer - NetworkManager handles spawning
            Debug.Log($"[PrefabSpawner] NetworkManager is active - disabling PrefabSpawner on {gameObject.name}.");
            enabled = false;
            return;
        }

        // Single player mode - continue normally
        respawnPoint = GetComponent<Transform>();
        if (spawnOnStart)
        {
            RespawnPrefab();
        }
    }

    public void RespawnPrefab()
    {
        // Safety check: Don't spawn if NetworkManager is active (multiplayer)
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            Debug.Log($"[PrefabSpawner] RespawnPrefab() called but NetworkManager is active - skipping spawn. Use NetworkPrefabSpawner for multiplayer.");
            return;
        }

        if (prefabToSpawn != null && respawnPoint != null)
        {
            // The 'transform' at the end sets the parent
            Instantiate(prefabToSpawn, respawnPoint.position, respawnPoint.rotation, transform);
            Debug.Log("Spawned Prefab: "+prefabToSpawn.name);
        }
        else
        {
            Debug.LogError("PrefabToSpawn or RespawnPoint is not assigned in the Inspector!");
        }
    }
}
