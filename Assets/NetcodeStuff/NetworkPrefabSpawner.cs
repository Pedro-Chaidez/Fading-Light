using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Network-aware prefab spawner that spawns prefabs with NetworkObject components.
/// Only the server/host can spawn networked prefabs.
/// </summary>
public class NetworkPrefabSpawner : MonoBehaviour
{
    [Header("Prefab Settings")]
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private bool spawnOnStart = false;

    [Header("Network Settings")]
    [SerializeField] private bool onlySpawnOnServer = true;

    private Transform spawnPoint;

    private void Start()
    {
        spawnPoint = transform;

        if (spawnOnStart)
        {
            SpawnPrefab();
        }
    }

    /// <summary>
    /// Spawns the prefab as a networked object (server/host only)
    /// </summary>
    public void SpawnPrefab()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("[NetworkPrefabSpawner] PrefabToSpawn is not assigned!");
            return;
        }

        // Check if we should spawn (server/host only)
        if (onlySpawnOnServer)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
            {
                Debug.Log("[NetworkPrefabSpawner] Not server/host - skipping spawn.");
                return;
            }
        }

        // Verify the prefab has a NetworkObject component
        NetworkObject networkObject = prefabToSpawn.GetComponent<NetworkObject>();
        if (networkObject == null)
        {
            Debug.LogError($"[NetworkPrefabSpawner] Prefab '{prefabToSpawn.name}' does not have a NetworkObject component! Use regular PrefabSpawner for non-networked prefabs.");
            return;
        }

        // Instantiate the prefab (don't set parent - NetworkObjects work best unparented)
        GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

        // Get the NetworkObject from the spawned instance
        NetworkObject spawnedNetworkObject = spawnedObject.GetComponent<NetworkObject>();
        if (spawnedNetworkObject != null)
        {
            // Spawn the networked object (this makes it networked)
            spawnedNetworkObject.Spawn();
            Debug.Log($"[NetworkPrefabSpawner] Spawned networked prefab: {prefabToSpawn.name} at {spawnPoint.position}");
        }
        else
        {
            Debug.LogError($"[NetworkPrefabSpawner] Spawned object '{spawnedObject.name}' lost its NetworkObject component!");
            Destroy(spawnedObject);
        }
    }

    /// <summary>
    /// Spawns the prefab at a specific position and rotation
    /// </summary>
    public void SpawnPrefabAt(Vector3 position, Quaternion rotation)
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("[NetworkPrefabSpawner] PrefabToSpawn is not assigned!");
            return;
        }

        // Check if we should spawn (server/host only)
        if (onlySpawnOnServer)
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
            {
                Debug.Log("[NetworkPrefabSpawner] Not server/host - skipping spawn.");
                return;
            }
        }

        // Verify the prefab has a NetworkObject component
        NetworkObject networkObject = prefabToSpawn.GetComponent<NetworkObject>();
        if (networkObject == null)
        {
            Debug.LogError($"[NetworkPrefabSpawner] Prefab '{prefabToSpawn.name}' does not have a NetworkObject component!");
            return;
        }

        // Instantiate and spawn
        GameObject spawnedObject = Instantiate(prefabToSpawn, position, rotation);
        NetworkObject spawnedNetworkObject = spawnedObject.GetComponent<NetworkObject>();
        
        if (spawnedNetworkObject != null)
        {
            spawnedNetworkObject.Spawn();
            Debug.Log($"[NetworkPrefabSpawner] Spawned networked prefab: {prefabToSpawn.name} at {position}");
        }
        else
        {
            Debug.LogError($"[NetworkPrefabSpawner] Spawned object lost its NetworkObject component!");
            Destroy(spawnedObject);
        }
    }

    /// <summary>
    /// Despawns all spawned networked objects that are children of this spawner
    /// </summary>
    public void DespawnAll()
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            Debug.LogWarning("[NetworkPrefabSpawner] Only server can despawn networked objects!");
            return;
        }

        // Find all NetworkObjects in the scene (we can't easily track children since we don't parent them)
        // This is a simple implementation - you might want to track spawned objects if needed
        Debug.Log("[NetworkPrefabSpawner] DespawnAll() called - implement object tracking if you need to despawn specific objects.");
    }
}
