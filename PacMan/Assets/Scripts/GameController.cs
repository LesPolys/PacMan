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
    [SerializeField] private List<Node> m_IntersectionPoints;
    [SerializeField] private TextMeshProUGUI m_ScoreText;
    
    private Dictionary<Vector3,Node> m_IntersectionNodeDictionary;
    private int m_score = 0;

    public int Score
    {
        get { return m_score; }
        set
        {
            m_score = value;
            UpdateScoreText();
        }
    }

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
}
