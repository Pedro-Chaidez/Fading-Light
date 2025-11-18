using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Generation Settings")]
    [SerializeField] private int totalRooms = 15;

    [Header("Grid Settings")]
    [SerializeField] private Vector3Int gridSize = new Vector3Int(10, 3, 10); // Width, Height (floors), Depth
    [SerializeField] private Vector3 cellSize = new Vector3(4f, 5f, 4f); // Size of each grid cell

    [Header("Room Prefabs")]
    [SerializeField] private List<Room> roomPrefabs = new List<Room>();
    [SerializeField] private Room spawnRoomPrefab;
    [SerializeField] private Room staircasePrefab;
    [SerializeField] private Room bigStaircasePrefab;

    [Header("Corridor Settings")]
    [SerializeField] private GameObject corridorPrefab;
    [SerializeField] private Vector3Int corridorSize = new Vector3Int(1, 1, 1);

    [Header("Generation Options")]
    [SerializeField] private bool generateOnStart = false;
    [SerializeField] private int seed = 0;
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int minStaircasesPerFloor = 1;

    private List<Room> allRooms = new List<Room>();
    private List<GameObject> allCorridors = new List<GameObject>();
    private System.Random randomGenerator;
    private bool[,,] occupiedCells; // 3D grid to track occupied cells

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
        InitializeGrid();

        // Place spawn room at center of lowest floor
        PlaceSpawnRoom();

        // Generate random rooms throughout the grid
        GenerateRooms();

        // Place staircases on each floor (except the top floor)
        PlaceStaircases();

        // Connect rooms using MST
        ConnectRoomsWithMST();

        Debug.Log($"Dungeon generated with {allRooms.Count} total rooms across {gridSize.y} floors!");
    }

    private void InitializeRandom()
    {
        if (useRandomSeed)
        {
            seed = Random.Range(0, int.MaxValue);
        }
        randomGenerator = new System.Random(seed);
    }

    private void InitializeGrid()
    {
        occupiedCells = new bool[gridSize.x, gridSize.y, gridSize.z];
    }

    private void PlaceSpawnRoom()
    {
        Vector3Int roomSize = spawnRoomPrefab.RoomSize;
        Vector3Int spawnGridPos = new Vector3Int(
            (gridSize.x - roomSize.x) / 2,
            0,
            (gridSize.z - roomSize.z) / 2
        );

        if (CanPlaceRoom(spawnRoomPrefab, spawnGridPos))
        {
            Vector3 spawnPosition = GridToWorldPosition(spawnGridPos);
            Room spawnRoom = InstantiateRoom(spawnRoomPrefab, spawnPosition, spawnGridPos);
            allRooms.Add(spawnRoom);
            MarkRoomCellsOccupied(spawnRoom, spawnGridPos);
        }
    }

    private void GenerateRooms()
    {
        int roomsPlaced = 0;
        int maxAttempts = totalRooms * 50; // Prevent infinite loops
        int attempts = 0;

        while (roomsPlaced < totalRooms && attempts < maxAttempts)
        {
            Room randomRoomPrefab = GetRandomRoomPrefab();
            Vector3Int gridPos = GetRandomGridPositionForRoom(randomRoomPrefab);

            if (CanPlaceRoom(randomRoomPrefab, gridPos))
            {
                Vector3 roomPosition = GridToWorldPosition(gridPos);
                Room newRoom = InstantiateRoom(randomRoomPrefab, roomPosition, gridPos);
                allRooms.Add(newRoom);
                MarkRoomCellsOccupied(newRoom, gridPos);
                roomsPlaced++;
            }

            attempts++;
        }

        if (roomsPlaced < totalRooms)
        {
            Debug.LogWarning($"Could only place {roomsPlaced} out of {totalRooms} rooms in the grid");
        }
    }

    private void PlaceStaircases()
    {
        if (gridSize.y <= 1) return;

        for (int floorY = 0; floorY < gridSize.y - 1; floorY++)
        {
            int staircasesPlaced = 0;
            int maxAttempts = 100;
            int attempts = 0;

            while (staircasesPlaced < minStaircasesPerFloor && attempts < maxAttempts)
            {
                Room staircasePrefabToUse = (randomGenerator.Next(0, 2) == 0 && bigStaircasePrefab != null)
                    ? bigStaircasePrefab
                    : staircasePrefab;

                if (staircasePrefabToUse != null)
                {
                    Vector3Int gridPos = GetRandomGridPositionForRoomOnFloor(staircasePrefabToUse, floorY);

                    if (CanPlaceRoom(staircasePrefabToUse, gridPos))
                    {
                        Vector3 staircasePosition = GridToWorldPosition(gridPos);
                        Room staircase = InstantiateRoom(staircasePrefabToUse, staircasePosition, gridPos);
                        allRooms.Add(staircase);
                        MarkRoomCellsOccupied(staircase, gridPos);
                        staircasesPlaced++;
                    }
                }

                attempts++;
            }

            if (staircasesPlaced < minStaircasesPerFloor)
            {
                Debug.LogWarning($"Could only place {staircasesPlaced} out of {minStaircasesPerFloor} staircases on floor {floorY}");
            }
        }
    }

    private bool CanPlaceRoom(Room roomPrefab, Vector3Int gridPos)
    {
        Vector3Int roomSize = roomPrefab.RoomSize;

        // Check if room fits within grid bounds
        for (int x = 0; x < roomSize.x; x++)
        {
            for (int y = 0; y < roomSize.y; y++)
            {
                for (int z = 0; z < roomSize.z; z++)
                {
                    Vector3Int checkPos = gridPos + new Vector3Int(x, y, z);

                    if (!IsWithinBounds(checkPos) || IsCellOccupied(checkPos))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private void MarkRoomCellsOccupied(Room room, Vector3Int gridPos)
    {
        Vector3Int roomSize = room.RoomSize;

        for (int x = 0; x < roomSize.x; x++)
        {
            for (int y = 0; y < roomSize.y; y++)
            {
                for (int z = 0; z < roomSize.z; z++)
                {
                    Vector3Int cellPos = gridPos + new Vector3Int(x, y, z);
                    MarkCellOccupied(cellPos);
                }
            }
        }
    }

    private void ConnectRoomsWithMST()
    {
        if (allRooms.Count < 2) return;

        // Separate rooms by floor
        for (int floorY = 0; floorY < gridSize.y; floorY++)
        {
            List<Room> roomsOnFloor = GetRoomsOnFloor(floorY);
            if (roomsOnFloor.Count < 2) continue;

            ConnectRoomsOnFloor(roomsOnFloor);
        }
    }

    private void ConnectRoomsOnFloor(List<Room> rooms)
    {
        // Build MST using Prim's algorithm
        List<Room> connectedRooms = new List<Room> { rooms[0] };
        List<Room> unconnectedRooms = new List<Room>(rooms);
        unconnectedRooms.RemoveAt(0);

        while (unconnectedRooms.Count > 0)
        {
            float shortestDistance = float.MaxValue;
            Room closestConnected = null;
            Room closestUnconnected = null;
            DoorLocation bestDoorA = null;
            DoorLocation bestDoorB = null;

            // Find the shortest connection between connected and unconnected rooms
            foreach (Room connectedRoom in connectedRooms)
            {
                foreach (Room unconnectedRoom in unconnectedRooms)
                {
                    // Find closest door pair
                    foreach (DoorLocation doorA in connectedRoom.DoorLocations)
                    {
                        Vector3 doorAPos = connectedRoom.GetDoorWorldPosition(doorA);

                        foreach (DoorLocation doorB in unconnectedRoom.DoorLocations)
                        {
                            Vector3 doorBPos = unconnectedRoom.GetDoorWorldPosition(doorB);
                            float distance = Vector3.Distance(doorAPos, doorBPos);

                            if (distance < shortestDistance)
                            {
                                shortestDistance = distance;
                                closestConnected = connectedRoom;
                                closestUnconnected = unconnectedRoom;
                                bestDoorA = doorA;
                                bestDoorB = doorB;
                            }
                        }
                    }
                }
            }

            // Create corridor between closest doors
            if (closestConnected != null && closestUnconnected != null && bestDoorA != null && bestDoorB != null)
            {
                CreateCorridor(closestConnected, closestUnconnected, bestDoorA, bestDoorB);
                connectedRooms.Add(closestUnconnected);
                unconnectedRooms.Remove(closestUnconnected);
            }
            else
            {
                break; // Can't connect any more rooms
            }
        }
    }

    private void CreateCorridor(Room roomA, Room roomB, DoorLocation doorA, DoorLocation doorB)
    {
        Vector3 startPos = roomA.GetDoorWorldPosition(doorA);
        Vector3 endPos = roomB.GetDoorWorldPosition(doorB);

        // Create L-shaped corridor (horizontal then vertical, or vice versa)
        List<Vector3> corridorPath = GenerateCorridorPath(startPos, endPos);

        foreach (Vector3 position in corridorPath)
        {
            Vector3Int gridPos = WorldToGridPosition(position);

            // Only place corridor if cell is not occupied by a room
            if (IsWithinBounds(gridPos) && !IsCellOccupied(gridPos))
            {
                GameObject corridor = Instantiate(corridorPrefab, position, Quaternion.identity, transform);
                corridor.name = $"Corridor_{allCorridors.Count}";
                allCorridors.Add(corridor);
                MarkCellOccupied(gridPos);
            }
        }
    }

    private List<Vector3> GenerateCorridorPath(Vector3 start, Vector3 end)
    {
        List<Vector3> path = new List<Vector3>();
        Vector3 current = start;

        // Move horizontally first (X axis)
        while (Mathf.Abs(current.x - end.x) > cellSize.x / 2f)
        {
            current.x += (end.x > current.x) ? cellSize.x : -cellSize.x;
            path.Add(current);
        }

        // Then move along Z axis
        while (Mathf.Abs(current.z - end.z) > cellSize.z / 2f)
        {
            current.z += (end.z > current.z) ? cellSize.z : -cellSize.z;
            path.Add(current);
        }

        return path;
    }

    private Room InstantiateRoom(Room prefab, Vector3 position, Vector3Int gridPos)
    {
        Room room = Instantiate(prefab, transform);
        room.Initialize(position, gridPos);
        room.name = $"{prefab.RoomName}_Floor{gridPos.y}_{allRooms.Count}";
        return room;
    }

    private Vector3Int GetRandomGridPosition()
    {
        int x = randomGenerator.Next(0, gridSize.x);
        int y = randomGenerator.Next(0, gridSize.y);
        int z = randomGenerator.Next(0, gridSize.z);
        return new Vector3Int(x, y, z);
    }

    private Vector3Int GetRandomGridPositionForRoom(Room roomPrefab)
    {
        Vector3Int roomSize = roomPrefab.RoomSize;

        // Ensure room doesn't exceed grid bounds
        int maxX = Mathf.Max(0, gridSize.x - roomSize.x);
        int maxY = Mathf.Max(0, gridSize.y - roomSize.y);
        int maxZ = Mathf.Max(0, gridSize.z - roomSize.z);

        int x = randomGenerator.Next(0, maxX + 1);
        int y = randomGenerator.Next(0, maxY + 1);
        int z = randomGenerator.Next(0, maxZ + 1);

        return new Vector3Int(x, y, z);
    }

    private Vector3Int GetRandomGridPositionOnFloor(int floorY)
    {
        int x = randomGenerator.Next(0, gridSize.x);
        int z = randomGenerator.Next(0, gridSize.z);
        return new Vector3Int(x, floorY, z);
    }

    private Vector3Int GetRandomGridPositionForRoomOnFloor(Room roomPrefab, int floorY)
    {
        Vector3Int roomSize = roomPrefab.RoomSize;

        // Ensure room doesn't exceed grid bounds
        int maxX = Mathf.Max(0, gridSize.x - roomSize.x);
        int maxZ = Mathf.Max(0, gridSize.z - roomSize.z);

        int x = randomGenerator.Next(0, maxX + 1);
        int z = randomGenerator.Next(0, maxZ + 1);

        return new Vector3Int(x, floorY, z);
    }

    private Vector3 GridToWorldPosition(Vector3Int gridPos)
    {
        return new Vector3(
            gridPos.x * cellSize.x,
            gridPos.y * cellSize.y,
            gridPos.z * cellSize.z
        );
    }

    private bool IsWithinBounds(Vector3Int gridPos)
    {
        return gridPos.x >= 0 && gridPos.x < gridSize.x &&
               gridPos.y >= 0 && gridPos.y < gridSize.y &&
               gridPos.z >= 0 && gridPos.z < gridSize.z;
    }

    private bool IsCellOccupied(Vector3Int gridPos)
    {
        if (!IsWithinBounds(gridPos)) return true;
        return occupiedCells[gridPos.x, gridPos.y, gridPos.z];
    }

    private void MarkCellOccupied(Vector3Int gridPos)
    {
        if (IsWithinBounds(gridPos))
        {
            occupiedCells[gridPos.x, gridPos.y, gridPos.z] = true;
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
        foreach (var room in allRooms)
        {
            if (room != null)
            {
                DestroyImmediate(room.gameObject);
            }
        }
        allRooms.Clear();

        foreach (var corridor in allCorridors)
        {
            if (corridor != null)
            {
                DestroyImmediate(corridor);
            }
        }
        allCorridors.Clear();

        // Destroy all children (safety cleanup)
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        if (occupiedCells != null)
        {
            occupiedCells = null;
        }
    }

    public Room GetRoomAtGridPosition(Vector3Int gridPos)
    {
        foreach (Room room in allRooms)
        {
            Vector3Int roomGridPos = room.GridPosition;
            Vector3Int roomSize = room.RoomSize;

            if (gridPos.x >= roomGridPos.x && gridPos.x < roomGridPos.x + roomSize.x &&
                gridPos.y >= roomGridPos.y && gridPos.y < roomGridPos.y + roomSize.y &&
                gridPos.z >= roomGridPos.z && gridPos.z < roomGridPos.z + roomSize.z)
            {
                return room;
            }
        }
        return null;
    }

    public Room GetRoomAtWorldPosition(Vector3 worldPos)
    {
        Vector3Int gridPos = WorldToGridPosition(worldPos);
        return GetRoomAtGridPosition(gridPos);
    }

    public Vector3Int WorldToGridPosition(Vector3 worldPos)
    {
        return new Vector3Int(
            Mathf.RoundToInt(worldPos.x / cellSize.x),
            Mathf.RoundToInt(worldPos.y / cellSize.y),
            Mathf.RoundToInt(worldPos.z / cellSize.z)
        );
    }

    public List<Room> GetRoomsOnFloor(int floorY)
    {
        List<Room> roomsOnFloor = new List<Room>();

        foreach (Room room in allRooms)
        {
            if (room.GridPosition.y == floorY)
            {
                roomsOnFloor.Add(room);
            }
        }

        return roomsOnFloor;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Draw grid bounds
        Gizmos.color = Color.yellow;
        Vector3 gridWorldSize = new Vector3(
            gridSize.x * cellSize.x,
            gridSize.y * cellSize.y,
            gridSize.z * cellSize.z
        );
        Gizmos.DrawWireCube(gridWorldSize / 2f, gridWorldSize);

        // Draw floor separators
        Gizmos.color = Color.cyan;
        for (int y = 0; y <= gridSize.y; y++)
        {
            Vector3 floorCenter = new Vector3(
                gridWorldSize.x / 2f,
                y * cellSize.y,
                gridWorldSize.z / 2f
            );
            Vector3 floorSize = new Vector3(gridWorldSize.x, 0.1f, gridWorldSize.z);
            Gizmos.DrawWireCube(floorCenter, floorSize);
        }

        // Draw occupied cells
        if (occupiedCells != null)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    for (int z = 0; z < gridSize.z; z++)
                    {
                        if (occupiedCells[x, y, z])
                        {
                            Gizmos.color = Color.red;
                            Vector3 cellCenter = GridToWorldPosition(new Vector3Int(x, y, z));
                            Gizmos.DrawWireCube(cellCenter, cellSize * 0.9f);
                        }
                    }
                }
            }
        }
    }
#endif
}