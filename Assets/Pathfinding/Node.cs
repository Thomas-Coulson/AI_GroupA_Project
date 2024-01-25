using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool m_walkable;
    public Vector3 m_worldPos;

    public Node(bool walkable, Vector3 worldPos)
    {
        m_walkable = walkable;
        m_worldPos = worldPos;
    }
}
