using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{
    [SerializeField] private float m_speed = 4.0f;
    [SerializeField]private Node m_currentNode;

    private const float MINIMUM_DISTANCE_TO_NODE = 0.25f;

    private Node m_previousNode;
    private Node m_targetNode;
    private Vector2 m_currentDirection = Vector2.zero;
    private Vector2 m_queuedDirection = Vector2.zero;

    private Node m_SpawnNode;

    public Vector2 CurrentDirection => m_currentDirection;

    private void Awake()
    {
        m_SpawnNode = m_currentNode;
    }

    void Start()
    {
        UpdateMovementTarget(Vector2.left); // always start left
    }

    void Update()
    {
        CheckInput();
        Move();
    }

    //tries to move the player towards the current target node
    //if the target is reached we attempt to immediatly set the new target using the queued direction of the player.
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

    //Provided a diretion, tries to set the new node target the player wishes to reach. If the opposite diretion to current facing is pressed the node is flipped back to the previous
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

    //Handles user input to determine new direction on next intersection
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

    //checks if the intersection we have reached as a valid neighbor we can move to using the provided direction
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

    //Handles moving the player between teleport nodes
    public void Teleport(Node connectedNode)
    {
        m_previousNode = connectedNode;
        m_currentNode = connectedNode;
        transform.localPosition = connectedNode.transform.localPosition;

        UpdateMovementTarget(m_currentDirection);
    }

    //handles colliding with ghosts
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ghost")
        {
            if (GameController.Instance.CurrentGhostMode == Ghost.GhostMode.Frightened)
            {
                other.GetComponent<Ghost>().Die();
                GameController.Instance.Score += 200;
            }
            else
            {
                transform.localPosition = m_SpawnNode.transform.localPosition;
                m_currentNode = m_SpawnNode;
                m_previousNode = m_currentNode;
                GameController.Instance.Die();
            }
        }
    }
}
