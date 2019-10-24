using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inky : Ghost
{
    public override void SetTarget()
    {
        Vector3 pacmanPosition = GameController.Instance.Player.transform.position;
        Vector3 pacmandirection = GameController.Instance.Player.CurrentDirection;
        Vector3 BlinkyPosition = GameController.Instance.Blinky.transform.position;
        Board gameboard = GameController.Instance.GameGrid;

        //As per the game, Inkys target tile is the tile two spaces in from of pacman + the direction vector from blinky to that tile.
        Tile twoSpacesInFront = GameController.Instance.GameGrid.TileAtWorldPosition(
            pacmanPosition + (pacmandirection * (gameboard.tileDiameter * 2)));

        Vector3 inkyDirection = twoSpacesInFront.position - BlinkyPosition;

        Tile inkyTarget = gameboard.TileAtWorldPosition(twoSpacesInFront.position + inkyDirection);
        m_ChaseTarget = inkyTarget.position;
        
        base.SetTarget();
    }
}
