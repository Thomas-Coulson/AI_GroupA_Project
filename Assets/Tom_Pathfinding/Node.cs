using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool m_walkable;
    public Vector3 m_worldPos;
    public int m_gridX, m_gridY;

    public Node m_parent;
    public int m_gCost, m_hCost;

    public int m_fCost
    {
        get
        {
            return m_gCost + m_hCost;
        }
    }

    public Node(bool walkable, Vector3 worldPos, int gridX, int gridY)
    {
        m_walkable = walkable;
        m_worldPos = worldPos;
        m_gridX = gridX;
        m_gridY = gridY;
    }

    
}