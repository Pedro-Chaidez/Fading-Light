using UnityEngine;
using System.Collections.Generic;

public class Floor
{
    private int floorLevel;
    private float heightOffset;
    private List<Room> rooms;
    
    public int FloorLevel => floorLevel;
    public float HeightOffset => heightOffset;
    public List<Room> Rooms => rooms;
    
    public Floor(int level, float height)
    {
        floorLevel = level;
        heightOffset = height;
        rooms = new List<Room>();
    }
    
    public void AddRoom(Room room)
    {
        if (room != null && !rooms.Contains(room))
        {
            rooms.Add(room);
        }
    }
    
    public void RemoveRoom(Room room)
    {
        if (rooms.Contains(room))
        {
            rooms.Remove(room);
        }
    }
    
    public void ClearRooms()
    {
        foreach (var room in rooms)
        {
            if (room != null)
            {
                Object.Destroy(room.gameObject);
            }
        }
        rooms.Clear();
    }
    
    public Room GetRoomAt(Vector3 position)
    {
        foreach (var room in rooms)
        {
            if (IsPositionInRoom(position, room))
            {
                return room;
            }
        }
        return null;
    }
    
    private bool IsPositionInRoom(Vector3 position, Room room)
    {
        Vector3 roomPos = room.transform.position;
        Vector3Int roomSize = room.RoomSize;
        
        return position.x >= roomPos.x - roomSize.x / 2f &&
               position.x <= roomPos.x + roomSize.x / 2f &&
               position.z >= roomPos.z - roomSize.y / 2f &&
               position.z <= roomPos.z + roomSize.y / 2f;
    }
}