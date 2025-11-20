using UnityEngine;
using System.Collections.Generic;

public class RoomConnectionManager : MonoBehaviour
{
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject hallwayPrefab;
    
    private Dictionary<Room, List<RoomConnection>> connections = new Dictionary<Room, List<RoomConnection>>();
    
    public void ConnectRooms(Room roomA, Room roomB, bool createHallway = true)
    {
        if (roomA == null || roomB == null) return;
        
        RoomConnection connection = new RoomConnection(roomA, roomB);
        
        AddConnection(roomA, connection);
        AddConnection(roomB, connection);
        
        if (createHallway)
        {
            CreateHallwayBetweenRooms(roomA, roomB);
        }
    }
    
    private void AddConnection(Room room, RoomConnection connection)
    {
        if (!connections.ContainsKey(room))
        {
            connections[room] = new List<RoomConnection>();
        }
        connections[room].Add(connection);
    }
    
    private void CreateHallwayBetweenRooms(Room roomA, Room roomB)
    {
        Vector3 startPos = roomA.transform.position;
        Vector3 endPos = roomB.transform.position;
        Vector3 midPoint = (startPos + endPos) / 2f;
        
        if (hallwayPrefab != null)
        {
            GameObject hallway = Instantiate(hallwayPrefab, midPoint, Quaternion.identity, transform);
            hallway.name = $"Hallway_{roomA.RoomName}_to_{roomB.RoomName}";
            
            // Orient hallway to face between rooms
            Vector3 direction = endPos - startPos;
            hallway.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
    
    public List<Room> GetConnectedRooms(Room room)
    {
        List<Room> connectedRooms = new List<Room>();
        
        if (connections.ContainsKey(room))
        {
            foreach (var connection in connections[room])
            {
                Room otherRoom = connection.GetOtherRoom(room);
                if (otherRoom != null)
                {
                    connectedRooms.Add(otherRoom);
                }
            }
        }
        
        return connectedRooms;
    }
    
    public void ClearConnections()
    {
        connections.Clear();
    }
}

[System.Serializable]
public class RoomConnection
{
    public Room roomA;
    public Room roomB;
    
    public RoomConnection(Room a, Room b)
    {
        roomA = a;
        roomB = b;
    }
    
    public Room GetOtherRoom(Room room)
    {
        if (room == roomA) return roomB;
        if (room == roomB) return roomA;
        return null;
    }
}