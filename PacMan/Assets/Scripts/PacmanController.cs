using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{
    [SerializeField] private float m_speed = 4.0f;
    [SerializeField]private Node m_currentNode;

    private const float MINIMUM_DISTANCE_TO_NODE = 0.1f;

    private Node m_previousNode;
    private Node m_targetNode;
    private Vector2 m_currentDirection = Vector2.zero;
    private Vector2 m_queuedDirection = Vector2.zero;
    private static GameController m_controller;
    
    void Start()
    {
        m_controller = GameController.Instance;
        UpdateMovementTarget(Vector2.left);
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        Move();
    }

    private void Move()
    {
        if (m_targetNode != m_currentNode)
        {
            if (Vector2.Distance(transform.position, m_targetNode.transform.position) <= MINIMUM_DISTANCE_TO_NODE)
            {
                m_currentNode = m_targetNode;
                
                transform.localPosition = m_targetNode.transform.localPosition;

                Node nextNode = ValidMove(m_queuedDirection);

                if (nextNode != null)
                {
                    m_currentDirection = m_queuedDirection;
                }

                if (nextNode == null)
                {
                    nextNode = ValidMove(m_currentDirection);
                }

                if (nextNode != null)
                {
                    m_targetNode = nextNode;
                    m_previousNode = m_currentNode;
                    m_currentNode = null;
                }
                else
                {
                    m_currentDirection = Vector2.zero;
                }

            } else {
                transform.localPosition += (Vector3) (m_currentDirection * m_speed) * Time.deltaTime;
            }
        }
    }

    private void UpdateMovementTarget(Vector2 direction)
    {
        if (direction != m_currentDirection)
        {
            m_queuedDirection = direction;

            if (direction == m_currentDirection*-1f)
            {
                m_currentDirection = direction;
                Node tempPreviousNode = m_previousNode;
                m_previousNode = m_targetNode;
                m_targetNode = tempPreviousNode;
                m_currentNode = null;
                return;
            }
        }

        if (m_currentNode != null)
        {
            Node nextNode = ValidMove(direction);
            if (nextNode != null)
            {
                m_currentDirection = direction;
                m_targetNode = nextNode;
                m_previousNode = m_currentNode;
                m_currentNode = null;
            }
        }
    }

    private void CheckInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            UpdateMovementTarget(Vector2.up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            UpdateMovementTarget(Vector2.down);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            UpdateMovementTarget(Vector2.left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            UpdateMovementTarget(Vector2.right);
        }
    }

    private Node ValidMove(Vector2 direction)
    {
        Node resultNode = null;

        for (int i = 0; i < m_currentNode.neighbors.Length; i++)
        {
            if (m_currentNode.validDirections[i] == direction)
            {
                resultNode = m_currentNode.neighbors[i];
            }
        }
        return resultNode;
    }


    public void Teleport(Node connectedNode)
    {
        m_previousNode = connectedNode;
        m_currentNode = connectedNode;
        transform.localPosition = connectedNode.transform.localPosition;

        UpdateMovementTarget(m_currentDirection);
    }
}
