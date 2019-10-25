using UnityEngine;

/*
 * Base class used by all ghosts
 * handles all ghost movement and mode changes
 * all child classes take over providing the target positon to use during the chase mode.
 */

public class Ghost : MonoBehaviour
{
    [SerializeField] protected float m_speed = 3.0f;
    [SerializeField] protected Node m_currentNode;
    [SerializeField] protected Transform m_ScatterTarget;
    [SerializeField] protected Vector3 m_ChaseTarget;
    [SerializeField] protected GhostMode m_currentMode = GhostMode.Chase;
    [SerializeField] protected Material m_NormalMaterial;
    [SerializeField] protected Material m_FrightnedMaterial;

    private Node m_SpawnNode;
    private bool m_IsDead;
    private float m_DeathTimer = 0;

    private const float MAX_DEATH_TIME = 5f;
    
    protected const float MINIMUM_DISTANCE_TO_NODE = 0.3f;

    protected Node m_previousNode;
    protected Node m_targetNode;
    protected Vector2 m_currentDirection = Vector2.zero;

    public GhostMode CurrentMode
    {
        get => m_currentMode;
        set
        {
            if (value == GhostMode.Frightened)
            {
                this.gameObject.GetComponent<Renderer>().material = m_FrightnedMaterial;
            }

            if (m_currentMode == GhostMode.Frightened && value != GhostMode.Frightened)
            {
                this.gameObject.GetComponent<Renderer>().material = m_NormalMaterial;
            }
            m_currentMode = value; 
        }
    }

    public enum GhostMode
    {
        Chase,
        Scatter,
        Frightened,
    }

    private void Awake()
    {
        m_SpawnNode = m_currentNode;
    }

    private void Start()
    {
        GameController.Instance.OnStateChange += HandleOnStateChange;
    }

    private void Update()
    {
        if (!m_IsDead)
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
            
            if (m_targetNode != null)
            {
                Move();
            } 
        }
        else
        {
            m_DeathTimer += Time.deltaTime;
            if (m_DeathTimer >= MAX_DEATH_TIME)
            {
                m_IsDead = false;
                m_DeathTimer = 0;
                this.gameObject.GetComponent<MeshRenderer>().enabled = true;
                this.gameObject.GetComponent<Collider>().enabled = true;
            }
        }
    }

    protected void Scatter()
    {
        if (m_targetNode == null)
        {
            MoveTowardsTarget(m_ScatterTarget.position);
        }
    }

    protected void Chase()
    {
        if (m_targetNode == null)
        {
            SetTarget();
            MoveTowardsTarget(m_ChaseTarget);
        }
    }

    protected void Frightened()
    {
        if (m_targetNode == null)
        {
            MoveTowardsTarget(GameController.Instance.GameGrid.GetRandomTile());
        }
    }

    //Checks to see if we have arrived close enough to our target node otherwise move towards it.
    private void Move()
    {
        if (Vector2.Distance(transform.position, m_targetNode.transform.position) <= MINIMUM_DISTANCE_TO_NODE)
        {
            transform.localPosition = m_targetNode.transform.localPosition;
            m_previousNode = m_currentNode;
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
        m_targetNode = m_previousNode;
        m_currentDirection = m_currentDirection *-1;
    }
    
    private void HandleOnStateChange(GhostMode newMode)
    {
        //ReverseDirection(); // causing issues wherin ghost will go through walls. Some node is not being set propperly.
        CurrentMode = newMode;
    }

    //Moves between teleport nodes
    public void Teleport(Node connectedNode)
    {
        transform.localPosition = connectedNode.transform.localPosition;
        m_previousNode = connectedNode;
        m_currentNode = connectedNode;
        m_targetNode = null;
    }

    //overidden by the child classes, sets the target for chase.
    public virtual void SetTarget()
    {
    }

    //Handle Ghost death
    public void Die()
    {
        m_IsDead = true;
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
        this.gameObject.GetComponent<Collider>().enabled = false;

        m_currentDirection = Vector2.zero;
        transform.localPosition = m_SpawnNode.transform.localPosition;
        m_currentNode = m_SpawnNode;
        m_targetNode = null;
    }
}
