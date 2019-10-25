using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *Nodes are are the structure we will use to keep track of intersections in the maze
 *node have a series of connected neighbors with an associated direction
 * to be queried by ghosts and player to move between locations.
 */
public class Node : MonoBehaviour
{
    public Node[] neighbors;
    public Vector2[] validDirections;
    
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
