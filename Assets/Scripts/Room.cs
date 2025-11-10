using UnityEngine;

[System.Serializable]
public class Room
{
    public int x;           // Position on X-axis (length)
    public int y;           // Position on Z-axis (width) - in 3D space this is typically Z
    public int width;       // Size along X-axis
    public int height;      // Size along Z-axis
    public int floorLevel;  // Which floor this room is on
    
    public Room(int x, int y, int width, int height, int floorLevel = 0)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.floorLevel = floorLevel;
    }
    
    public Vector2Int GetCenter()
    {
        return new Vector2Int(x + width / 2, y + height / 2);
    }
    
    public Vector3 GetCenter3D(float floorSpacing)
    {
        Vector2Int center2D = GetCenter();
        return new Vector3(center2D.x, floorLevel * floorSpacing, center2D.y);
    }
    
    public bool Overlaps(Room other, int buffer = 0)
    {
        // Only check overlap if on the same floor
        if (floorLevel != other.floorLevel)
            return false;
            
        return x < other.x + other.width + buffer &&
               x + width + buffer > other.x &&
               y < other.y + other.height + buffer &&
               y + height + buffer > other.y;
    }
    
    public bool Contains(Vector2Int point)
    {
        return point.x >= x && point.x < x + width &&
               point.y >= y && point.y < y + height;
    }
    
    public Vector2Int GetRandomPointInRoom()
    {
        return new Vector2Int(
            Random.Range(x + 1, x + width - 1),
            Random.Range(y + 1, y + height - 1)
        );
    }
    
    public int GetArea()
    {
        return width * height;
    }
    
    public Vector2Int GetBottomLeft()
    {
        return new Vector2Int(x, y);
    }
    
    public Vector2Int GetTopRight()
    {
        return new Vector2Int(x + width, y + height);
    }
    
    public int GetFloorLevel()
    {
        return floorLevel;
    }
}