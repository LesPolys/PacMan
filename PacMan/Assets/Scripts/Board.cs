using UnityEngine;

 /*
 *The Board lays the whole game as a set of tiles.
 *Allows us to queuery the board for positions so we can pass them the ghosts as targets.
 *Allows us to keep track of positions in an orderly fashion when not using the node system 
 */
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

    //populates the grid and assigns wethere or not the space is occupied by a wall or open
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

    //takes a worldspace position and return the matching tile
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

    //gets us a random tile, to be used with frightened state as a location to target
    public Vector3 GetRandomTile()
    {
        return grid[Random.Range(0,gridSizeX), Random.Range(0,gridSizeY)].position;
    }
}
