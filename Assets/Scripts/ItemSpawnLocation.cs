using UnityEngine;

public class ItemSpawnLocation : MonoBehaviour
{
    [SerializeField]
    private GameObject prefabToSpawn; 
    private Transform respawnPoint;

    private void Awake()
    {
        respawnPoint = GetComponent<Transform>();
    }

    public void RespawnPrefab()
    {
        if (prefabToSpawn != null && respawnPoint != null)
        {
            Instantiate(prefabToSpawn, respawnPoint.position, respawnPoint.rotation);
            Debug.Log("Spawned item: "+prefabToSpawn.name);
        }
        else
        {
            Debug.LogError("PrefabToSpawn or RespawnPoint is not assigned in the Inspector!");
        }
    }
}
