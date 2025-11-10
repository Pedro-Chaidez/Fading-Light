using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon Dimensions")]
    [SerializeField] private int dungeonLength = 20;  // X-axis
    [SerializeField] private int dungeonWidth = 20;   // Z-axis
    [SerializeField] private int dungeonHeight = 2;   // Y-axis (number of floors)
    [SerializeField] private float floorSpacing = 1f; // Vertical distance between floors
    
    [Header("Room Settings")]
    [SerializeField] private int numberOfRoomsPerFloor = 10;
    [SerializeField] private int minRoomLength = 1;
    [SerializeField] private int maxRoomLength = 5;
    [SerializeField] private int minRoomWidth = 1;
    [SerializeField] private int maxRoomWidth = 3;
    [SerializeField] private int roomHeightInUnits = 4; // Height of each room
    
    [Header("Prefabs")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject ceilingPrefab;
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject stairsPrefab;
    
    [Header("Generation")]
    [SerializeField] private int maxAttempts = 100;
    [SerializeField] private bool generateStairsBetweenFloors = true;
    
    private List<Room>[] roomsByFloor; // Array of room lists, one per floor
    private int[,,] dungeonGrid; // 3D grid: [x, z, y]
    
    // Grid values
    private const int EMPTY = 0;
    private const int FLOOR = 1;
    private const int WALL = 2;
    private const int DOOR = 3;
    private const int CEILING = 4;
    private const int STAIRS = 5;
    
    void Start()
    {
        GenerateDungeon();
    }
    
    public void GenerateDungeon()
    {
        // Initialize grid with 3D dimensions
        dungeonGrid = new int[dungeonLength, dungeonWidth, dungeonHeight];
        roomsByFloor = new List<Room>[dungeonHeight];
        
        for (int i = 0; i < dungeonHeight; i++)
        {
            roomsByFloor[i] = new List<Room>();
        }
        
        // Clear existing dungeon
        ClearDungeon();
        
        // Generate each floor
        for (int floor = 0; floor < dungeonHeight; floor++)
        {
            GenerateFloor(floor);
        }
        
        // Connect floors with stairs
        if (generateStairsBetweenFloors && dungeonHeight > 1)
        {
            ConnectFloors();
        }
        
        // Instantiate the dungeon
        InstantiateDungeon();
    }
    
    private void GenerateFloor(int floorLevel)
    {
        // Generate rooms for this floor
        GenerateRooms(floorLevel);
        
        // Connect rooms with corridors
        ConnectRooms(floorLevel);
        
        // Add walls around rooms and corridors
        AddWalls(floorLevel);
        
        // Place doors
        PlaceDoors(floorLevel);
    }
    
    private void GenerateRooms(int floorLevel)
    {
        for (int i = 0; i < numberOfRoomsPerFloor; i++)
        {
            int attempts = 0;
            bool roomPlaced = false;
            
            while (!roomPlaced && attempts < maxAttempts)
            {
                int roomLength = Random.Range(minRoomLength, maxRoomLength + 1);
                int roomWidth = Random.Range(minRoomWidth, maxRoomWidth + 1);
                int roomX = Random.Range(1, dungeonLength - roomLength - 1);
                int roomZ = Random.Range(1, dungeonWidth - roomWidth - 1);
                
                Room newRoom = new Room(roomX, roomZ, roomLength, roomWidth, floorLevel);
                
                if (!DoesRoomOverlap(newRoom, floorLevel))
                {
                    roomsByFloor[floorLevel].Add(newRoom);
                    CarveRoom(newRoom, floorLevel);
                    roomPlaced = true;
                }
                
                attempts++;
            }
        }
    }
    
    private bool DoesRoomOverlap(Room newRoom, int floorLevel)
    {
        foreach (Room room in roomsByFloor[floorLevel])
        {
            if (newRoom.Overlaps(room, 2)) // 2 tile buffer between rooms
            {
                return true;
            }
        }
        return false;
    }
    
    private void CarveRoom(Room room, int floorLevel)
    {
        for (int x = room.x; x < room.x + room.width; x++)
        {
            for (int z = room.y; z < room.y + room.height; z++)
            {
                if (x >= 0 && x < dungeonLength && z >= 0 && z < dungeonWidth)
                {
                    dungeonGrid[x, z, floorLevel] = FLOOR;
                }
            }
        }
    }
    
    private void ConnectRooms(int floorLevel)
    {
        List<Room> rooms = roomsByFloor[floorLevel];
        
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            Room roomA = rooms[i];
            Room roomB = rooms[i + 1];
            
            Vector2Int centerA = roomA.GetCenter();
            Vector2Int centerB = roomB.GetCenter();
            
            // Create L-shaped corridor
            if (Random.value > 0.5f)
            {
                CreateHorizontalCorridor(centerA.x, centerB.x, centerA.y, floorLevel);
                CreateVerticalCorridor(centerA.y, centerB.y, centerB.x, floorLevel);
            }
            else
            {
                CreateVerticalCorridor(centerA.y, centerB.y, centerA.x, floorLevel);
                CreateHorizontalCorridor(centerA.x, centerB.x, centerB.y, floorLevel);
            }
        }
    }
    
    private void CreateHorizontalCorridor(int x1, int x2, int z, int floorLevel)
    {
        int start = Mathf.Min(x1, x2);
        int end = Mathf.Max(x1, x2);
        
        for (int x = start; x <= end; x++)
        {
            if (x >= 0 && x < dungeonLength && z >= 0 && z < dungeonWidth)
            {
                if (dungeonGrid[x, z, floorLevel] == EMPTY)
                {
                    dungeonGrid[x, z, floorLevel] = FLOOR;
                }
            }
        }
    }
    
    private void CreateVerticalCorridor(int z1, int z2, int x, int floorLevel)
    {
        int start = Mathf.Min(z1, z2);
        int end = Mathf.Max(z1, z2);
        
        for (int z = start; z <= end; z++)
        {
            if (x >= 0 && x < dungeonLength && z >= 0 && z < dungeonWidth)
            {
                if (dungeonGrid[x, z, floorLevel] == EMPTY)
                {
                    dungeonGrid[x, z, floorLevel] = FLOOR;
                }
            }
        }
    }
    
    private void AddWalls(int floorLevel)
    {
        for (int x = 0; x < dungeonLength; x++)
        {
            for (int z = 0; z < dungeonWidth; z++)
            {
                if (dungeonGrid[x, z, floorLevel] == FLOOR)
                {
                    // Check surrounding tiles (only horizontal, not diagonal for cleaner walls)
                    int[] dx = { -1, 1, 0, 0 };
                    int[] dz = { 0, 0, -1, 1 };
                    
                    for (int i = 0; i < 4; i++)
                    {
                        int checkX = x + dx[i];
                        int checkZ = z + dz[i];
                        
                        if (checkX >= 0 && checkX < dungeonLength && checkZ >= 0 && checkZ < dungeonWidth)
                        {
                            if (dungeonGrid[checkX, checkZ, floorLevel] == EMPTY)
                            {
                                dungeonGrid[checkX, checkZ, floorLevel] = WALL;
                            }
                        }
                    }
                }
            }
        }
    }
    
    private void PlaceDoors(int floorLevel)
    {
        List<Room> rooms = roomsByFloor[floorLevel];
        
        foreach (Room room in rooms)
        {
            // Check room perimeter for potential door locations
            for (int x = room.x; x < room.x + room.width; x++)
            {
                CheckAndPlaceDoor(x, room.y - 1, floorLevel); // Bottom edge
                CheckAndPlaceDoor(x, room.y + room.height, floorLevel); // Top edge
            }
            
            for (int z = room.y; z < room.y + room.height; z++)
            {
                CheckAndPlaceDoor(room.x - 1, z, floorLevel); // Left edge
                CheckAndPlaceDoor(room.x + room.width, z, floorLevel); // Right edge
            }
        }
    }
    
    private void CheckAndPlaceDoor(int x, int z, int floorLevel)
    {
        if (x >= 0 && x < dungeonLength && z >= 0 && z < dungeonWidth)
        {
            if (dungeonGrid[x, z, floorLevel] == WALL)
            {
                // Check if this wall connects a room to a corridor
                int floorCount = 0;
                
                if (x > 0 && dungeonGrid[x - 1, z, floorLevel] == FLOOR) floorCount++;
                if (x < dungeonLength - 1 && dungeonGrid[x + 1, z, floorLevel] == FLOOR) floorCount++;
                if (z > 0 && dungeonGrid[x, z - 1, floorLevel] == FLOOR) floorCount++;
                if (z < dungeonWidth - 1 && dungeonGrid[x, z + 1, floorLevel] == FLOOR) floorCount++;
                
                // Place door if wall connects two floor tiles
                if (floorCount >= 2)
                {
                    dungeonGrid[x, z, floorLevel] = DOOR;
                }
            }
        }
    }
    
    private void ConnectFloors()
    {
        for (int floor = 0; floor < dungeonHeight - 1; floor++)
        {
            if (roomsByFloor[floor].Count > 0 && roomsByFloor[floor + 1].Count > 0)
            {
                // Pick a random room on each floor
                Room lowerRoom = roomsByFloor[floor][Random.Range(0, roomsByFloor[floor].Count)];
                Room upperRoom = roomsByFloor[floor + 1][Random.Range(0, roomsByFloor[floor + 1].Count)];
                
                // Place stairs in the lower room
                Vector2Int stairPos = lowerRoom.GetRandomPointInRoom();
                dungeonGrid[stairPos.x, stairPos.y, floor] = STAIRS;
                
                // Place stairs in the upper room (ideally near the same position)
                Vector2Int upperStairPos = upperRoom.GetRandomPointInRoom();
                dungeonGrid[upperStairPos.x, upperStairPos.y, floor + 1] = STAIRS;
            }
        }
    }
    
    private void InstantiateDungeon()
    {
        for (int floor = 0; floor < dungeonHeight; floor++)
        {
            float yPosition = floor * floorSpacing;
            
            for (int x = 0; x < dungeonLength; x++)
            {
                for (int z = 0; z < dungeonWidth; z++)
                {
                    Vector3 position = new Vector3(x, yPosition, z);
                    
                    switch (dungeonGrid[x, z, floor])
                    {
                        case FLOOR:
                            if (floorPrefab != null)
                                Instantiate(floorPrefab, position, Quaternion.identity, transform);
                            
                            // Add ceiling if there's a floor above
                            if (floor < dungeonHeight - 1 && ceilingPrefab != null)
                            {
                                Vector3 ceilingPos = position + Vector3.up * (roomHeightInUnits - 1);
                                Instantiate(ceilingPrefab, ceilingPos, Quaternion.identity, transform);
                            }
                            break;
                            
                        case WALL:
                            if (wallPrefab != null)
                            {
                                // Create vertical wall
                                for (int h = 0; h < roomHeightInUnits; h++)
                                {
                                    Vector3 wallPos = position + Vector3.up * h;
                                    Instantiate(wallPrefab, wallPos, Quaternion.identity, transform);
                                }
                            }
                            break;
                            
                        case DOOR:
                            if (floorPrefab != null)
                                Instantiate(floorPrefab, position, Quaternion.identity, transform);
                            
                            if (doorPrefab != null)
                            {
                                Vector3 doorPos = position + Vector3.up * 0.5f;
                                GameObject door = Instantiate(doorPrefab, doorPos, Quaternion.identity, transform);
                                RotateDoor(door, x, z, floor);
                            }
                            break;
                            
                        case STAIRS:
                            if (floorPrefab != null)
                                Instantiate(floorPrefab, position, Quaternion.identity, transform);
                            
                            if (stairsPrefab != null)
                            {
                                Instantiate(stairsPrefab, position, Quaternion.identity, transform);
                            }
                            break;
                    }
                }
            }
        }
    }
    
    private void RotateDoor(GameObject door, int x, int z, int floorLevel)
    {
        bool horizontalConnection = false;
        
        if (x > 0 && x < dungeonLength - 1)
        {
            if (dungeonGrid[x - 1, z, floorLevel] == FLOOR && dungeonGrid[x + 1, z, floorLevel] == FLOOR)
            {
                horizontalConnection = true;
            }
        }
        
        if (!horizontalConnection)
        {
            door.transform.rotation = Quaternion.Euler(0, 90, 0);
        }
    }
    
    private void ClearDungeon()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    
    public List<Room>[] GetAllRooms()
    {
        return roomsByFloor;
    }
    
    public List<Room> GetRoomsOnFloor(int floor)
    {
        if (floor >= 0 && floor < dungeonHeight)
        {
            return roomsByFloor[floor];
        }
        return null;
    }
    
    public int GetDungeonLength() { return dungeonLength; }
    public int GetDungeonWidth() { return dungeonWidth; }
    public int GetDungeonHeight() { return dungeonHeight; }
}