using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pinky : Ghost
{
    public override void SetTarget()
    {
        Vector3 pacmanPosition = GameController.Instance.Player.transform.position;
        Vector3 pacmandirection = GameController.Instance.Player.CurrentDirection;
        Board gameboard = GameController.Instance.GameGrid;

        //As per the game, Pinky target tile is the tile four spaces in from of pacman
        Tile twoSpacesInFront = GameController.Instance.GameGrid.TileAtWorldPosition(
            pacmanPosition + (pacmandirection * (gameboard.tileDiameter * 4)));

        m_ChaseTarget = twoSpacesInFront.position;
        base.SetTarget();
    }
}
