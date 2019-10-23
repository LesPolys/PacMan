﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node[] neighbors;

    public Vector2[] validDirections;
    
    // Start is called before the first frame update
    void Start()
    {
        validDirections = new Vector2[neighbors.Length];

        for (int i = 0; i < neighbors.Length; i++)
        {
            Node neighbor = neighbors[i];
            Vector2 direction = neighbor.transform.localPosition - transform.localPosition;

            validDirections[i] = direction.normalized;
        }
    }
}