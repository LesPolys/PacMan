using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField]protected float m_speed = 3.0f;
    [SerializeField]protected Node m_currentNode;
    [SerializeField] protected Transform m_ScatterTarget;
    [SerializeField] protected Transform m_ChaseTarget;

    protected const float MINIMUM_DISTANCE_TO_NODE = 0.1f;

    protected Node m_previousNode;
    protected Node m_targetNode;
    protected Vector2 m_currentDirection = Vector2.zero;
    
    protected enum GhostMode
    {
        Chase,
        Scatter,
        Frightened,
        Reverse
    }

    protected GhostMode m_currentMode = GhostMode.Frightened;
    protected GhostMode m_nextMode;
    
    private void Update()
    {
        if (m_currentMode == GhostMode.Scatter)
        {
            Scatter();
        }

        if (m_currentMode == GhostMode.Chase)
        {
            Chase();
        }
        
        if (m_currentMode == GhostMode.Frightened)
        {
            Frightened();
        }
        
    }

    protected void Scatter()
    {
        if (m_targetNode == null)
        {
            MoveTowardsTarget(m_ScatterTarget.position);
        }

        if (m_targetNode != null)
        {
            Move();
        }
    }

    protected void Chase()
    {
        if (m_targetNode == null)
        {
            MoveTowardsTarget(m_ChaseTarget.position);
        }

        if (m_targetNode != null)
        {
            Move();
        }
    }

    protected void Frightened()
    {
        if (m_targetNode == null)
        {
            MoveTowardsTarget(GameController.Instance.GameGrid.GetRandomTile());
        }

        if (m_targetNode != null)
        {
            Move();
        }
    }

    private void Move()
    {
        if (Vector2.Distance(transform.position, m_targetNode.transform.position) <= MINIMUM_DISTANCE_TO_NODE)
        {
            transform.localPosition = m_targetNode.transform.localPosition;
            m_currentNode = m_targetNode;
            m_targetNode = null;
        }
        else
        {
            transform.localPosition += (Vector3) (m_currentDirection * m_speed) * Time.deltaTime;
        } 
    }

    private void MoveTowardsTarget(Vector3 target)
    {
        Vector2 currentBest = Vector2.zero;
        Vector2 currentDirection = Vector2.zero;

        foreach (Vector2 direction in m_currentNode.validDirections)
        {
            Tile adjacentTile = GameController.Instance.GameGrid.TileAtWorldPosition(
                m_currentNode.transform.position + (new Vector3(direction.x,direction.y, 0) * GameController.Instance.GameGrid.tileDiameter));
            if (direction != -1*m_currentDirection)
            {
                if (currentBest == Vector2.zero )
                {
                    currentBest = adjacentTile.position;
                    currentDirection = direction;
                }
                else
                {
                    if (Vector2.Distance(adjacentTile.position, target) < Vector2.Distance(currentBest, target))
                    {
                        currentBest = adjacentTile.position;
                        currentDirection = direction;
                    }
                }  
            }
        }
        
        for (int i = 0; i < m_currentNode.neighbors.Length; i++)
        {
            if (m_currentNode.validDirections[i] == currentDirection)
            {
                m_targetNode = m_currentNode.neighbors[i];
                m_currentDirection = currentDirection;
                return;
            }
        }
    }

    //Too be called any time a ghost changes states. causing it to reverse direction
    private void ReverseDirection()
    {
        m_currentDirection = m_currentDirection *-1;
        Node tempPreviousNode = m_previousNode;
        m_previousNode = m_targetNode;
        m_targetNode = tempPreviousNode;
        m_currentNode = null;
    }


    public void Teleport(Node connectedNode)
    {
        transform.localPosition = connectedNode.transform.localPosition;
        m_previousNode = connectedNode;
        m_currentNode = connectedNode;
        m_targetNode = null;
    }
}
