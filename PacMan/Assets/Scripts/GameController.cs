using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private static GameController _instance;

    public static GameController Instance
    {
        get { return _instance; }
    }

    [SerializeField] private TileGrid m_gameGrid;

    public TileGrid GameGrid
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
