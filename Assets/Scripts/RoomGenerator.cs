using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [Header("Room Component Prefabs")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject ceilingPrefab;
    [SerializeField] private GameObject basicWallPrefab;
    [SerializeField] private GameObject doorWallPrefab;
    
    [Header("Room Generation Settings")]
    [SerializeField] private Vector3 roomPosition = new Vector3(-4f, 0f, 7f);
    [SerializeField] private bool generateOnStart = true;
    
    [Header("Room Structure")]
    private GameObject generatedRoom;
    
    void Start()
    {
        if (generateOnStart)
        {
            GenerateSpawnRoom();
        }
    }

    /// <summary>
    /// Generates the spawn room using individual prefab components
    /// </summary>
    public void GenerateSpawnRoom()
    {
        // Create parent room object
        generatedRoom = new GameObject("Generated Spawn Room");
        generatedRoom.transform.position = roomPosition;
        
        // Generate floor
        GenerateFloor();
        
        // Generate walls
        GenerateWalls();
        
        // Generate ceiling
        GenerateCeiling();
        
        Debug.Log($"Spawn room generated at position: {roomPosition}");
    }

    private void GenerateFloor()
    {
        if (floorPrefab == null)
        {
            Debug.LogError("Floor prefab is not assigned!");
            return;
        }

        GameObject floor = Instantiate(floorPrefab, generatedRoom.transform);
        floor.transform.localPosition = Vector3.zero;
        floor.name = "Floor";
    }

    private void GenerateWalls()
    {
        // North wall (Basic Wall) - at z=7 relative to room, rotated 180 degrees
        if (basicWallPrefab != null)
        {
            GameObject northWall = Instantiate(basicWallPrefab, generatedRoom.transform);
            northWall.transform.localPosition = new Vector3(-3f, 0f, 7f);
            northWall.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            northWall.name = "North Wall (Basic)";
        }

        // South wall (Door Wall) - at z=3
        if (doorWallPrefab != null)
        {
            GameObject southWall = Instantiate(doorWallPrefab, generatedRoom.transform);
            southWall.transform.localPosition = new Vector3(-2f, 0f, 3f);
            southWall.transform.localRotation = Quaternion.identity;
            southWall.name = "South Wall (Door)";
        }

        // West wall (Door Wall) - at x=-6, rotated 90 degrees
        if (doorWallPrefab != null)
        {
            GameObject westWall = Instantiate(doorWallPrefab, generatedRoom.transform);
            westWall.transform.localPosition = new Vector3(-6f, 0f, 7f);
            westWall.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
            westWall.name = "West Wall (Door)";
        }

        // East wall (Door Wall) - at x=2, rotated 270 degrees
        if (doorWallPrefab != null)
        {
            GameObject eastWall = Instantiate(doorWallPrefab, generatedRoom.transform);
            eastWall.transform.localPosition = new Vector3(2f, 0f, 7f);
            eastWall.transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
            eastWall.name = "East Wall (Door)";
        }
    }

    private void GenerateCeiling()
    {
        if (ceilingPrefab == null)
        {
            Debug.LogError("Ceiling prefab is not assigned!");
            return;
        }

        GameObject ceiling = Instantiate(ceilingPrefab, generatedRoom.transform);
        ceiling.transform.localPosition = new Vector3(0f, 5f, 9f);
        ceiling.name = "Ceiling";
    }

    /// <summary>
    /// Clears the currently generated room
    /// </summary>
    public void ClearGeneratedRoom()
    {
        if (generatedRoom != null)
        {
            DestroyImmediate(generatedRoom);
            Debug.Log("Generated room cleared.");
        }
    }

    /// <summary>
    /// Returns the generated room GameObject
    /// </summary>
    public GameObject GetGeneratedRoom()
    {
        return generatedRoom;
    }
}