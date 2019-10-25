using System;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _instance;
    public static GameController Instance
    {
        get { return _instance; }
    }

    [SerializeField] private Board m_gameGrid;
    [SerializeField] private TextMeshProUGUI m_ScoreText;
    [SerializeField] private TextMeshProUGUI m_EndgameText;
    [SerializeField] private TextMeshProUGUI m_LivesText;
    [SerializeField] private GameObject m_Inky;
    [SerializeField] private GameObject m_Blinky;
    [SerializeField] private GameObject m_Pinky;
    [SerializeField] private GameObject m_Clyde;
    [SerializeField] private PacmanController m_Player;
    [SerializeField] private float[] m_GhostModeTimes = new float[7];
    
    private const float PINKY_TIMER_LIMIT = 3f;
    private const float INKY_SCORE_LIMIT = 30;
    private const float CLYDE_SCORE_LIMIT = 244/3;
    private const float MAX_FIGHTENED_TIME = 5f;
    
    private bool m_isPinkySpawned = false;
    private bool m_isInkySpawned = false;
    private bool m_isClydeSpawned = false;
    private int m_Score = 0;
    private int m_PelletsCollected = 0;
    private int m_Lives = 3;
    private float m_Timer;
    private float m_FrightenedTimer;
    private bool m_IsFrightened = false;
    private int m_GhostModeIndex = 0;
    private Ghost.GhostMode m_CurrentGhostMode = Ghost.GhostMode.Chase;
    
    public event Action<Ghost.GhostMode> OnStateChange;

    //when pellets collected reaches a certain threshold (all pellets) we win 
    public int PelletCount
    {
        get => m_PelletsCollected;
        set
        {
            m_PelletsCollected++;
            if (PelletCount == 244)
            {
                Win();
            }
            Score++;
        } 
    }
    
    //When score is changed updated the text display
    public int Score
    {
        get { return m_Score; }
        set
        {
            m_Score = value;
            UpdateScoreText();
        }
    }

    public PacmanController Player => m_Player;
    public GameObject Inky => m_Inky;
    public GameObject Blinky => m_Blinky;
    public GameObject Pinky => m_Pinky;
    public GameObject Clyde => m_Clyde;
    public Ghost.GhostMode CurrentGhostMode => m_CurrentGhostMode;
    
    private void UpdateScoreText()
    {
        m_ScoreText.text = Score.ToString();
    }

    //Quick access to GameGrid for querying locations maybe should be an instanced item
    public Board GameGrid
    {
        get => m_gameGrid;
    }
    
    private void Awake()
    {
        //create game instance
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        
        //inital setting of lives text
        m_LivesText.text = m_Lives.ToString();
    }

    //Handles timers for game states. Only two real persistance states, frightened or not. Ghosts are released and scatter/chases are toggled betwee otherwise
    private void Update()
    {
        if (!m_IsFrightened) // we dont want to increment or change ghost modes while frightened
        {
            m_Timer += Time.deltaTime;
        
            UnlockGhosts();
            ToggleChase();
        }
        else
        {
            m_FrightenedTimer += Time.deltaTime;
            if (m_FrightenedTimer >= MAX_FIGHTENED_TIME) //once the timer has elapsed well return ghosts to normal state.
            {
                m_IsFrightened = false;
                m_FrightenedTimer = 0;
                //change out of frightened state
            }
        }
    }

    //As time and score increases we unlock the appropriate ghost
    private void UnlockGhosts()
    {
        if (m_Timer >= PINKY_TIMER_LIMIT && !m_isPinkySpawned)
        {
            m_isPinkySpawned = true;
            m_Pinky.SetActive(true);
            Pinky.GetComponent<Ghost>().CurrentMode = m_CurrentGhostMode;
        }

        if (PelletCount >= INKY_SCORE_LIMIT && !m_isInkySpawned)
        {
            m_isInkySpawned = true;
            m_Inky.SetActive(true);
            m_Inky.GetComponent<Ghost>().CurrentMode = m_CurrentGhostMode;
        }

        if (PelletCount >= CLYDE_SCORE_LIMIT && !m_isClydeSpawned)
        {
            m_isClydeSpawned = true;
            m_Clyde.SetActive(true);
            m_Clyde.GetComponent<Ghost>().CurrentMode = m_CurrentGhostMode;
        }
    }

    //toggle between chase and scatter mode based on the wave timer
    private void ToggleChase()
    {
        if (m_GhostModeIndex < m_GhostModeTimes.Length && m_Timer >= m_GhostModeTimes[m_GhostModeIndex])
        {
            //toggle between chase and scatter
            m_GhostModeIndex++;
            if (m_GhostModeIndex % 2 == 0)
            {
                OnStateChange(Ghost.GhostMode.Chase);
                m_CurrentGhostMode = Ghost.GhostMode.Chase;
            }
            else
            {
                OnStateChange(Ghost.GhostMode.Scatter);
                m_CurrentGhostMode = Ghost.GhostMode.Scatter;
            }
            m_Timer = 0;
        }
    }

    // change to frightened mode    
    public void ChangeToFrightened()
    {
        OnStateChange(Ghost.GhostMode.Frightened);
        m_CurrentGhostMode = Ghost.GhostMode.Frightened;
        m_IsFrightened = true;
    }

    //When player dies, decrease life count, if too low. game over
    public void Die()
    {
        m_Lives--;
        if (m_Lives <= 0)
        {
            Lose();
        }
        m_LivesText.text = m_Lives.ToString();
    }
    
    //Pause game timescale and display win
    private void Win()
    {
        Time.timeScale = 0f;
        m_EndgameText.text = "YOU WIN!";
    }

    //pause game timescale and display lose
    private void Lose()
    {
        Time.timeScale = 0f;
        m_EndgameText.text = "YOU LOSE!"; 
    }

}
