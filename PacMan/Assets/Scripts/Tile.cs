using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public bool isWall;
    public Vector3 position;
    public Vector2 gridIndex;

    public Tile(bool _isWall, Vector3 gridPoint, Vector2 _gridIndex)
    {
        isWall = _isWall;
        position = gridPoint;
        gridIndex = _gridIndex;
    }
}
