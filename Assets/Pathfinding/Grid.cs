using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    Node[,] m_grid;
    public Vector2 m_gridWorldSize;
    public float m_nodeSize;

    public LayerMask m_unwalkableMask;

    int m_gridSizeX, m_gridSizeY;

    private void Start()
    {
        m_gridSizeX = Mathf.RoundToInt(m_gridWorldSize.x / (m_nodeSize * 2)); //number of nodes in grid X
        m_gridSizeY = Mathf.RoundToInt(m_gridWorldSize.y / (m_nodeSize * 2)); //number of nodes in grid Y
        CreateGrid();
    }

    private void CreateGrid()
    {
        m_grid = new Node[m_gridSizeX, m_gridSizeY];

        //start at bottom left
        Vector3 bottomLeftWorld = transform.position - (Vector3.right * (m_gridSizeX / 2)) - (Vector3.forward * (m_gridSizeY / 2));

        for(int x = 0; x < m_gridSizeX; x++)
        {
            for(int y = 0; y < m_gridSizeY; y++)
            {
                //find world position of each node position
                Vector3 currentWorldPos = bottomLeftWorld + Vector3.right * (x * m_nodeSize * 2 + m_nodeSize) + Vector3.forward * (y * m_nodeSize * 2 + m_nodeSize);

                //mark if node is walkable
                bool walkable = !(Physics.CheckSphere(currentWorldPos, m_nodeSize, m_unwalkableMask));

                //add node to grid
                m_grid[x, y] = new Node(walkable, currentWorldPos);
            }
        }
    }

    public Node GridPosFromWorld(Vector3 worldPos)
    {
        float percentX = (worldPos.x + m_gridWorldSize.x / 2) / m_gridWorldSize.x; //give percentage of X axis where node is
        float percentY = (worldPos.z + m_gridWorldSize.y / 2) / m_gridWorldSize.y; //give percentage of Y axis where node is

        //always clamp these values to be valid inside of the grid
        percentX = Mathf.Clamp01(percentX);
        percentX = Mathf.Clamp01(percentX);

        //find grid coordinates based off of percentage
        int x = Mathf.RoundToInt((m_gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((m_gridSizeY - 1) * percentY);

        return m_grid[x, y];
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(m_gridWorldSize.x, 1, m_gridWorldSize.y)); //changing 2D gridsize to Vector3

        if(m_grid != null)
        {
            foreach(Node node in m_grid)
            {
                Gizmos.color = node.m_walkable ? Color.white : Color.red;
                Gizmos.DrawCube(node.m_worldPos, Vector3.one * (m_nodeSize * 2 - .05f));
            }
        }
    }

}
