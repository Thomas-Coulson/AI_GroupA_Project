using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool m_walkable;
    public Vector3 m_worldPos;
    public int m_gridX, m_gridY;

    public Node m_parent;
    public int m_gCost, m_hCost;

    int m_heapIndex;

    public int m_fCost
    {
        get
        {
            return m_gCost + m_hCost;
        }
    }

    public int HeapIndex
    {
        get { return m_heapIndex; }
        set { m_heapIndex = value; }
    }

    public Node(bool walkable, Vector3 worldPos, int gridX, int gridY)
    {
        m_walkable = walkable;
        m_worldPos = worldPos;
        m_gridX = gridX;
        m_gridY = gridY;
    }

    public int CompareTo(Node nodeToCompare)
    {
        //return 1 if f cost is lower (highger priority), and -1 if higher (lower priority)
        int compare = m_fCost.CompareTo(nodeToCompare.m_fCost);
        if(compare == 0)
        {
            compare = m_hCost.CompareTo(nodeToCompare.m_hCost);
        }
        return -compare;
    }

}
