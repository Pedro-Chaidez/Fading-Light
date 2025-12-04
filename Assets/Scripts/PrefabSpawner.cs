using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabToSpawn; 
    private Transform respawnPoint;
    [SerializeField]
    private bool spawnOnStart;

    private void Start()
    {
        respawnPoint = GetComponent<Transform>();
        if (spawnOnStart)
        {
            RespawnPrefab();
        }
    }

    public void RespawnPrefab()
    {
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
