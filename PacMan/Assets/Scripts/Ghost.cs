using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ghost : MonoBehaviour
{
    [SerializeField]protected float m_speed = 3.0f;
    [SerializeField]protected Node m_currentNode;
    [SerializeField] protected Vector3 m_ScatterTarget;

    protected const float MINIMUM_DISTANCE_TO_NODE = 0.1f;

    protected Node m_previousNode;
    protected Node m_targetNode;
    protected Vector2 m_currentDirection = Vector2.zero;
    protected static GameController m_controller;
    
    protected enum GhostMode
    {
        Chase,
        Scatter,
        Frightened
    }

    protected GhostMode m_currentMode;
    
    void Start()
    {
        m_controller = GameController.Instance;
    }

    private void Update()
    {
        if (m_currentMode == GhostMode.Scatter)
        {
            Scatter();
        }
        
    }

    public abstract void Chase();

    protected void Scatter()
    {

        if( Vector2.Distance(transform.position, m_targetNode.transform.position) <= MINIMUM_DISTANCE_TO_NODE)
        {
            transform.localPosition = m_targetNode.transform.localPosition;
            m_currentNode = m_targetNode;
            m_currentDirection = Vector2.zero;
            m_targetNode = null;
        }


    }
    public abstract void Frightened();
}
