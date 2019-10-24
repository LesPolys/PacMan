using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public LayerMask wallMask;
    public LayerMask intersectionMask;
    public Vector2 gridSize;
    public float tileRadius;
    private Tile[,] grid;

    public float tileDiameter;
    int gridSizeX, gridSizeY;

    void Start()
    {
        tileDiameter = tileRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridSize.x / tileDiameter);
        gridSizeY = Mathf.RoundToInt(gridSize.y / tileDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Tile[gridSizeX,gridSizeY];
        Vector3 leftCorner = transform.position - Vector3.right * gridSize.x / 2 - Vector3.up * gridSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 gridPoint = leftCorner + Vector3.right * (x * tileDiameter + tileRadius) + Vector3.up * (y * tileDiameter + tileRadius);
                bool isWall = !(Physics.CheckSphere(gridPoint, tileRadius - 0.2f, wallMask));
                grid[x,y] = new Tile(isWall, gridPoint, new Vector2(x,y));
            }
        }
    }

    public Tile TileAtWorldPosition(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridSize.x / 2) / gridSize.x;
        float percentY = (worldPosition.y + gridSize.y / 2) / gridSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, gridSize.y, 0));
        if (grid != null)
        {
            foreach (Tile tile in grid)
            {
                Gizmos.color = (!tile.isWall) ? Color.blue : Color.white;
                Gizmos.DrawCube(tile.position, Vector3.one * tileDiameter);
            }
        }
    }

    public Vector3 GetRandomTile()
    {
        return grid[Random.Range(0,gridSizeX), Random.Range(0,gridSizeY)].position;
    }
}
