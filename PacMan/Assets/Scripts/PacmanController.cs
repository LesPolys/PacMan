using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{
    // Start is called before the first frame update
    private static GameController m_controller;
    
    void Awake()
    {
        m_controller = GameController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(m_controller.GameGrid.TileAtWorldPosition(transform.position).gridIndex);
    }
}
