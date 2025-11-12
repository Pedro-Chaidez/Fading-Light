using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField] private int numberOfFloors = 3;
    [SerializeField] private int roomsPerFloor = 5;
    [SerializeField] private float floorHeight = 5f;
    
    [Header("Room Prefabs")]
    [SerializeField] private List<Room> roomPrefabs = new List<Room>();
    [SerializeField] private Room spawnRoomPrefab;
    [SerializeField] private Room staircasePrefab;
    
    [Header("Generation Options")]
    [SerializeField] private bool generateOnStart = false;
    [SerializeField] private int seed = 0;
    [SerializeField] private bool useRandomSeed = true;
    
    private List<Floor> generatedFloors = new List<Floor>();
    private System.Random randomGenerator;
    
    private void Start()
    {
        if (generateOnStart)
        {
            GenerateDungeon();
        }
    }
    
    public void GenerateDungeon()
    {
        ClearDungeon();
        InitializeRandom();
        
        for (int floorIndex = 0; floorIndex < numberOfFloors; floorIndex++)
        {
            Floor newFloor = new Floor(floorIndex, floorHeight);
            GenerateFloor(newFloor, floorIndex);
            generatedFloors.Add(newFloor);
        }
        
        ConnectFloorsWithStaircases();
        
        Debug.Log($"Dungeon generated with {numberOfFloors} floors!");
    }
    
    private void InitializeRandom()
    {
        if (useRandomSeed)
        {
            seed = Random.Range(0, int.MaxValue);
        }
        randomGenerator = new System.Random(seed);
    }
    
    private void GenerateFloor(Floor floor, int floorIndex)
    {
        // First room is always the spawn room on floor 0
        if (floorIndex == 0)
        {
            Vector3 spawnPosition = new Vector3(0, floorIndex * floorHeight, 0);
            Room spawnRoom = InstantiateRoom(spawnRoomPrefab, spawnPosition, floorIndex);
            floor.AddRoom(spawnRoom);
        }
        
        // Generate additional rooms
        for (int i = 0; i < roomsPerFloor; i++)
        {
            Room randomRoomPrefab = GetRandomRoomPrefab();
            Vector3 roomPosition = CalculateRoomPosition(floor, i);
            
            Room newRoom = InstantiateRoom(randomRoomPrefab, roomPosition, floorIndex);
            floor.AddRoom(newRoom);
        }
    }
    
    private Room InstantiateRoom(Room prefab, Vector3 position, int floorIndex)
    {
        Room room = Instantiate(prefab, transform);
        room.Initialize(position, floorIndex);
        room.name = $"{prefab.RoomName}_Floor{floorIndex}";
        return room;
    }
    
    private Vector3 CalculateRoomPosition(Floor floor, int roomIndex)
    {
        // Simple grid layout - you can make this more sophisticated
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(roomsPerFloor));
        int x = (roomIndex % gridSize) * 12; // 12 units spacing
        int z = (roomIndex / gridSize) * 12;
        
        return new Vector3(x, floor.FloorLevel * floorHeight, z);
    }
    
    private void ConnectFloorsWithStaircases()
    {
        for (int i = 0; i < generatedFloors.Count - 1; i++)
        {
            Floor currentFloor = generatedFloors[i];
            Floor nextFloor = generatedFloors[i + 1];
            
            // Place staircase between floors
            Vector3 staircasePosition = new Vector3(6, i * floorHeight, 6);
            Room staircase = InstantiateRoom(staircasePrefab, staircasePosition, i);
            currentFloor.AddRoom(staircase);
        }
    }
    
    private Room GetRandomRoomPrefab()
    {
        if (roomPrefabs.Count == 0)
        {
            Debug.LogError("No room prefabs assigned!");
            return null;
        }
        
        int randomIndex = randomGenerator.Next(0, roomPrefabs.Count);
        return roomPrefabs[randomIndex];
    }
    
    public void ClearDungeon()
    {
        foreach (var floor in generatedFloors)
        {
            floor.ClearRooms();
        }
        generatedFloors.Clear();
        
        // Destroy all children
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
    
    public Floor GetFloor(int floorIndex)
    {
        if (floorIndex >= 0 && floorIndex < generatedFloors.Count)
        {
            return generatedFloors[floorIndex];
        }
        return null;
    }
}