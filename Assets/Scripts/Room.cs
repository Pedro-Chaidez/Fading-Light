using UnityEngine;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    [Header("Room Configuration")]
    [SerializeField] private Vector3Int roomSize;
    [SerializeField] private string roomName = "Untitled Room";

    [Header("Room Components")]
    [SerializeField] private Transform floorParent;
    [SerializeField] private Transform ceilingParent;
    [SerializeField] private Transform wallsParent;

    [Header("Door Locations")]
    [SerializeField] private List<DoorLocation> doorLocations = new List<DoorLocation>();

    public Vector3Int RoomSize => roomSize;
    public string RoomName => roomName;
    public List<DoorLocation> DoorLocations => doorLocations;

    private Vector3 worldPosition;

    public void Initialize(Vector3 position, int floorLevel)
    {
        worldPosition = position;
        transform.position = new Vector3(position.x, floorLevel * 5f, position.z);
    }

    public Vector3 GetDoorWorldPosition(DoorLocation door)
    {
        return transform.position + door.localPosition;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(roomSize.x, roomSize.y, roomSize.z));

        // Draw door locations
        Gizmos.color = Color.green;
        foreach (var door in doorLocations)
        {
            Vector3 doorPos = transform.position + door.localPosition;
            Gizmos.DrawWireSphere(doorPos, 0.5f);
        }
    }
#endif
}

[System.Serializable]
public class DoorLocation
{
    public string doorName;
    public Vector3 localPosition;
    public DoorDirection direction;
}

public enum DoorDirection
{
    North,
    South,
    East,
    West
}