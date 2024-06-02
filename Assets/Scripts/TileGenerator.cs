using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGenerator : MonoBehaviour
{
    private Tilemap tilemap;
    [SerializeField] private RuleTile ruleTile;

    private void Awake()
    {
        this.transform.localScale = new Vector3(Mathf.RoundToInt(this.transform.localScale.x), Mathf.RoundToInt(this.transform.localScale.y), 1f);
        this.tilemap = FindObjectOfType<Tilemap>();
        //DrawTiles();
    }

    public void DrawTiles()
    {
        //Debug.Log("draw tiles");
        Vector3 position = transform.position;
        Vector3 scale = transform.localScale;

        // Calculate the bounds based on the position and scale
        Vector3Int min = tilemap.WorldToCell(position - (scale / 2));
        Vector3Int max = tilemap.WorldToCell(position + (scale / 2));

        // Loop through each cell within the bounds and set the RuleTile
        for (int x = min.x; x < max.x; x++)
        {
            for (int y = min.y; y < max.y; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                tilemap.SetTile(tilePosition, ruleTile);
            }
        }
    }
}
