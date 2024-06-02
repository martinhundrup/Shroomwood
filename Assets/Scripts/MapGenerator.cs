using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    //[SerializeField] private GameObject roomPrefab; // The prefab for the rooms
    //[SerializeField] private int numberOfRooms = 10; // Number of rooms to generate

    //private void Start()
    //{

    //}

    //private void PlaceRoom()
    //{

    //}

    public GameObject roomPrefab; // The prefab for the rooms
    public int numberOfRooms = 10; // Number of rooms to generate
    public int roomWidth = 1; // Size of each room (assumed to be square)
    [SerializeField] private int roomHeight = 1;

    private List<Vector2Int> roomPositions = new List<Vector2Int>();
    private HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>();
    private List<GameObject> instantiatedRooms = new List<GameObject>();
    private Queue<Vector2Int> availableRooms = new Queue<Vector2Int>();
    private Dictionary<Vector2Int, GameObject> roomMap = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int furthestRoom;

    [SerializeField] Tilemap tilemap;
    [SerializeField] RuleTile ruleTile;


    private void Draw()
    {
        ClearMap();
        GenerateMap();
        DrawRoomTiles();
        furthestRoom = FindFurthestRoom(Vector2Int.zero);
        //GameObject furthestRoomTile = GetRoomTile(furthestRoom);
        //furthestRoomTile.GetComponent<SpriteRenderer>().color = Color.red;
        //GetRoomTile(Vector2Int.zero).GetComponent<SpriteRenderer>().color = Color.green;
    }
    void Start()
    {
        Draw();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Draw();
        }
    }

    void GenerateMap()
    {
        // Clear previous room positions
        roomPositions.Clear();
        occupiedPositions.Clear();
        availableRooms.Clear();
        roomMap.Clear();

        // Place the starting room at (0, 0)
        Vector2Int startPosition = Vector2Int.zero;
        roomPositions.Add(startPosition);
        occupiedPositions.Add(startPosition);
        availableRooms.Enqueue(startPosition);
        InstantiateRoom(startPosition);

        // Generate remaining rooms
        for (int i = 1; i < numberOfRooms; i++)
        {
            if (availableRooms.Count > 0)
            {
                PlaceAdjacentRoom();
            }
        }
    }

    void PlaceAdjacentRoom()
    {
        // Randomly select an existing room from the queue
        Vector2Int currentRoom = availableRooms.Dequeue();

        // Find all possible adjacent positions
        List<Vector2Int> possiblePositions = new List<Vector2Int>
        {
            currentRoom + Vector2Int.up,
            currentRoom + Vector2Int.down,
            currentRoom + Vector2Int.left,
            currentRoom + Vector2Int.right
        };

        // Shuffle the possible positions to randomize placement
        for (int i = 0; i < possiblePositions.Count; i++)
        {
            Vector2Int temp = possiblePositions[i];
            int randomIndex = Random.Range(i, possiblePositions.Count);
            possiblePositions[i] = possiblePositions[randomIndex];
            possiblePositions[randomIndex] = temp;
        }

        // Place the room in the first available position
        foreach (var position in possiblePositions)
        {
            if (!occupiedPositions.Contains(position))
            {
                roomPositions.Add(position);
                occupiedPositions.Add(position);
                availableRooms.Enqueue(position);
                InstantiateRoom(position);
                break;
            }
        }
    }

    void InstantiateRoom(Vector2Int position)
    {
        Vector3 worldPosition = new Vector3(position.x * roomWidth, position.y * roomHeight, 0);
        GameObject room = Instantiate(roomPrefab, worldPosition, Quaternion.identity, transform);
        
        //tilemap.SetTile(new Vector3Int(position.x, position.y, 0), ruleTile);
        instantiatedRooms.Add(room);
        roomMap[position] = room;
    }

    private void DrawRoomTiles()
    {
        foreach (var position in roomPositions)
        {
            bool up = roomMap.ContainsKey(position + Vector2Int.up);
            bool down = roomMap.ContainsKey(position + Vector2Int.down);
            bool left = roomMap.ContainsKey(position + Vector2Int.left);
            bool right = roomMap.ContainsKey(position + Vector2Int.right);
            roomMap[position].GetComponent<Room>().GenerateWalls(up, down, left, right);
        }
    }

    void ClearMap()
    {
        //tilemap.ClearAllTiles();
        foreach (GameObject room in instantiatedRooms)
        {
            Destroy(room);
        }
        instantiatedRooms.Clear();
    }

    Vector2Int FindFurthestRoom(Vector2Int start)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();

        queue.Enqueue(start);
        distances[start] = 0;

        Vector2Int furthestRoom = start;
        int maxDistance = 0;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int currentDistance = distances[current];

            List<Vector2Int> possiblePositions = new List<Vector2Int>
            {
                current + Vector2Int.up,
                current + Vector2Int.down,
                current + Vector2Int.left,
                current + Vector2Int.right
            };

            foreach (var position in possiblePositions)
            {
                if (occupiedPositions.Contains(position) && !distances.ContainsKey(position))
                {
                    distances[position] = currentDistance + 1;
                    queue.Enqueue(position);

                    if (distances[position] > maxDistance)
                    {
                        maxDistance = distances[position];
                        furthestRoom = position;
                    }
                }
            }
        }

        return furthestRoom;
    }

    GameObject GetRoomTile(Vector2Int position)
    {
        roomMap.TryGetValue(position, out GameObject room);
        return room;
    }
}
