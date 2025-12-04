using UnityEngine;

public class Respawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private PrefabSpawner playerSpawner;
    void Start()
    {
        playerSpawner = GetComponent<PrefabSpawner>();
        playerSpawner.RespawnPrefab();
    }
}
