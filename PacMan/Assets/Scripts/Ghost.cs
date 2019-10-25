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

    public GhostMode CurrentMode //if our property is updated between frightened or not then we want to set our material accordinly
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

    //ghosts can only be in the chase, scatter or frightened states
    public enum GhostMode 
    {
        Chase,
        Scatter,
        Frightened,
    }

    //grab our first node so we can use it as a spawn point if the ghost is to die
    private void Awake()
    {
        m_SpawnNode = m_currentNode;
    }

    //subscribe to the gamecontroller to see when states are updated
    private void Start()
    {
        GameController.Instance.OnStateChange += HandleOnStateChange;
    }

    //Handle movement and target selection based on current state
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
            //Hide the dead ghost for a specific time before re-displaying them
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

    //Each ghost has its own scatter target in their respective corner
    //as the ghost approaches its scatter target it will become locked into a loop around an area
    protected void Scatter()
    {
        if (m_targetNode == null)
        {
            MoveTowardsTarget(m_ScatterTarget.position);
        }
    }

    //when Chasing each ghost overrides SetTarget in order to create its own unqiue behaviour
    protected void Chase()
    {
        if (m_targetNode == null)
        {
            SetTarget();
            MoveTowardsTarget(m_ChaseTarget);
        }
    }

    //When frightened ghosts target a random tile on the board
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

    //As per https://www.gamasutra.com/view/feature/132330/the_pacman_dossier.php
    //when a ghost arrives at an intersection they look at all tiles adjacent to the intersection that they could move
    //ghosts can never move backwards though
    //they then compare this adjacent tile to the target tile they are trying to reach
    //the tile selected is the one physically closest to the target tile
    //they then select that direction.
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
