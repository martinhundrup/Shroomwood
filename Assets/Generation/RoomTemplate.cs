using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class RoomTemplate : ScriptableObject
{
    [SerializeField] private  Vector2Int roomSize;
    private Tilemap tilemap;
    private TileBase[] tiles;
    private Vector2 roomPosition;

    public Tilemap Tilemap
    {
        get { return tilemap; }
        set { tilemap = value; }
    }
    public Vector2Int RoomSize
    {
        get { return roomSize; }
    }
    public TileBase[] Tiles
    {
        get { return tiles; }
        set { tiles = value; }
    }
    public Vector2 RoomPosition
    {
        get { return roomPosition; }
    }

    public void DrawTiles()
    {

    }
}
public class RoomDesignerWindow : EditorWindow
{
    private RoomTemplate currentRoomTemplate;

    private TileBase defaultTile;

    [MenuItem("Window/Room Designer")]
    public static void ShowWindow()
    {
        GetWindow<RoomDesignerWindow>("Room Designer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Room Designer", EditorStyles.boldLabel);

        RoomTemplate newRoomTemplate = (RoomTemplate)EditorGUILayout.ObjectField("Room Template", currentRoomTemplate, typeof(RoomTemplate), false);
        Tilemap newTilemap = (Tilemap)EditorGUILayout.ObjectField("Tilemap", currentRoomTemplate.Tilemap, typeof(Tilemap), true);
        defaultTile = (TileBase)EditorGUILayout.ObjectField("Default Tile", defaultTile, typeof(TileBase), false);

        if (newRoomTemplate != currentRoomTemplate || newTilemap != newRoomTemplate.Tilemap)
        {
            currentRoomTemplate = newRoomTemplate;
            newRoomTemplate.Tilemap = newTilemap;
            DrawRoom();
        }

        if (currentRoomTemplate != null && newRoomTemplate.Tilemap != null)
        {
            if (GUILayout.Button("Clear Room"))
            {
                ClearRoom();
            }

            if (GUILayout.Button("Save Room"))
            {
                SaveRoom();
            }

            DrawTilemapEditor();
        }
    }

    private void DrawRoom()
    {
        if (currentRoomTemplate == null || currentRoomTemplate.Tilemap == null)
            return;

        currentRoomTemplate.Tilemap.ClearAllTiles();

        int gridWidth = currentRoomTemplate.RoomSize.x;
        int gridHeight = currentRoomTemplate.RoomSize.y;

        if (currentRoomTemplate.Tiles == null || currentRoomTemplate.Tiles.Length != gridWidth * gridHeight)
        {
            currentRoomTemplate.Tiles = new TileBase[gridWidth * gridHeight];
        }

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int index = y * gridWidth + x;
                TileBase tile = currentRoomTemplate.Tiles[index];
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                currentRoomTemplate.Tilemap.SetTile(tilePosition, tile);
            }
        }
    }

    private void DrawTilemapEditor()
    {
        if (currentRoomTemplate == null || currentRoomTemplate.Tilemap == null)
            return;

        // Define the size of the grid
        int gridWidth = currentRoomTemplate.RoomSize.x;
        int gridHeight = currentRoomTemplate.RoomSize.y;
        float tileSize = 20f; // Smaller size for better fit

        // Ensure the tiles array matches the room size
        if (currentRoomTemplate.Tiles == null || currentRoomTemplate.Tiles.Length != gridWidth * gridHeight)
        {
            currentRoomTemplate.Tiles = new TileBase[gridWidth * gridHeight];
        }

        // Create a scroll view in case the grid is larger than the window
        Vector2 scrollPos = EditorGUILayout.BeginScrollView(Vector2.zero, GUILayout.Width(position.width), GUILayout.Height(position.height - 100));

        for (int y = gridHeight - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < gridWidth; x++)
            {
                int index = y * gridWidth + x;
                TileBase tile = currentRoomTemplate.Tiles[index];

                // Change the button color based on whether a tile is placed or not
                GUI.backgroundColor = tile != null ? Color.green : Color.red;

                // Create a button for each tile
                if (GUILayout.Button("", GUILayout.Width(tileSize), GUILayout.Height(tileSize)))
                {
                    Vector3Int tilePosition = new Vector3Int(x, y, 0);
                    TileBase currentTile = currentRoomTemplate.Tilemap.GetTile(tilePosition);

                    // Toggle tile placement
                    if (currentTile == defaultTile)
                    {
                        currentRoomTemplate.Tilemap.SetTile(tilePosition, null);
                        currentRoomTemplate.Tiles[index] = null;
                    }
                    else
                    {
                        currentRoomTemplate.Tilemap.SetTile(tilePosition, defaultTile);
                        currentRoomTemplate.Tiles[index] = defaultTile;
                    }

                    // Mark the template as dirty to ensure it gets saved
                    EditorUtility.SetDirty(currentRoomTemplate);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        // Reset GUI background color to default
        GUI.backgroundColor = Color.white;
    }

    private void ClearRoom()
    {
        currentRoomTemplate.Tilemap.ClearAllTiles();
        for (int i = 0; i < currentRoomTemplate.Tiles.Length; i++)
        {
            currentRoomTemplate.Tiles[i] = null;
        }
    }

    private void SaveRoom()
    {
        for (int y = 0; y < currentRoomTemplate.Tilemap.size.y; y++)
        {
            for (int x = 0; x < currentRoomTemplate.Tilemap.size.x; x++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                currentRoomTemplate.Tiles[y * currentRoomTemplate.Tilemap.size.x + x] = currentRoomTemplate.Tilemap.GetTile(tilePosition);
            }
        }

        EditorUtility.SetDirty(currentRoomTemplate);
        AssetDatabase.SaveAssets();
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (currentRoomTemplate != null && currentRoomTemplate.Tilemap != null)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            Event e = Event.current;

            if (e.type == EventType.MouseDown && e.button == 0)
            {
                Vector2 mousePosition = e.mousePosition;
                Vector3 worldPosition = HandleUtility.GUIPointToWorldRay(mousePosition).origin;
                Vector3Int cellPosition = currentRoomTemplate.Tilemap.WorldToCell(worldPosition);

                if (currentRoomTemplate.Tilemap.GetTile(cellPosition) != defaultTile)
                {
                    currentRoomTemplate.Tilemap.SetTile(cellPosition, defaultTile);
                }
                else
                {
                    currentRoomTemplate.Tilemap.SetTile(cellPosition, null);
                }

                e.Use();
            }
        }
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
}