using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clyde : Ghost
{
    public override void SetTarget()
    {
        float minimunDistance = GameController.Instance.GameGrid.tileDiameter * 8;
        Tile pacmanCurrentTile =
            GameController.Instance.GameGrid.TileAtWorldPosition(GameController.Instance.Player.transform.position);

        if (Vector3.Distance(transform.position, pacmanCurrentTile.position) <= minimunDistance)
        {
            //run away from player
            m_ChaseTarget = m_ScatterTarget.position;
        }
        else
        {
            //follow player
            m_ChaseTarget = pacmanCurrentTile.position;
        }
        
        base.SetTarget();
    }
}
