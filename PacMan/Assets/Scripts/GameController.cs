using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject m_Inky;
    [SerializeField] private GameObject m_Blinky;
    [SerializeField] private GameObject m_Pinky;
    [SerializeField] private GameObject m_Clyde;
    [SerializeField] private PacmanController m_Player;
    [SerializeField] private float[] m_GhostModeTimes = new float[7];
    
    private const float PINKY_TIMER_LIMIT = 3f;
    private const float INKY_SCORE_LIMIT = 30;
    private const float CLYDE_SCORE_LIMIT = 300/3;
    private const float MAX_FIGHTENED_TIME = 5f;
    
    private Dictionary<Vector3,Node> m_IntersectionNodeDictionary;
    private int m_score = 0;
    private float m_Timer;
    private float m_FrightenedTimer;
    private bool m_IsFrightened = false;
    private int m_GhostModeIndex = 0;

    public int Score
    {
        get { return m_score; }
        set
        {
            m_score = value;
            UpdateScoreText();
        }
    }

    public PacmanController Player => m_Player;
    public GameObject Inky => m_Inky;
    public GameObject Blinky => m_Blinky;
    public GameObject Pinky => m_Pinky;
    public GameObject Clyde => m_Clyde;


    private void UpdateScoreText()
    {
        m_ScoreText.text = Score.ToString();
    }

    public Board GameGrid
    {
        get => m_gameGrid;
    }
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }

    private void Update()
    {
        if (!m_IsFrightened)
        {
            m_Timer += Time.deltaTime;
        
            UnlockGhosts();
            ToggleChase();
        }
        else
        {
            m_FrightenedTimer += Time.deltaTime;
            if (m_FrightenedTimer >= MAX_FIGHTENED_TIME)
            {
                m_IsFrightened = false;
                m_FrightenedTimer = 0;
                //change out of frightened state
            }
        }

    }

    private void UnlockGhosts()
    {
        if (m_Timer >= PINKY_TIMER_LIMIT)
        {
            m_Pinky.SetActive(true);
        }

        if (Score >= INKY_SCORE_LIMIT)
        {
            m_Inky.SetActive(true);
        }

        if (Score >= CLYDE_SCORE_LIMIT)
        {
            m_Clyde.SetActive(true);
        }
        
    }

    private void ToggleChase()
    {
        if (m_Timer >= m_GhostModeTimes[m_GhostModeIndex])
        {
            //toggle the chase
            m_GhostModeIndex++;
            m_Timer = 0;
        }
        
    }
    
}
