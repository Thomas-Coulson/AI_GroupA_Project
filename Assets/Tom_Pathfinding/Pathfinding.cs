using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public enum EPathfindingType
{
    AStar,
    BestFirst
}

public class AStarPathfinding : MonoBehaviour
{

    Grid m_grid;

    public EPathfindingType m_pathfindingType;

    private void Awake()
    {
        m_grid = GetComponent<Grid>();
    }

    public Vector3[] StartPathfinding(Vector3 startPos, Vector3 targetPos)
    {
        return FindPath(startPos, targetPos);
    }

    Vector3[] FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //debug timer
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSucess = false;

        Node startNode = m_grid.GridPosFromWorld(startPos);
        Node targetNode = m_grid.GridPosFromWorld(targetPos);

        if(startNode.m_walkable && targetNode.m_walkable)
        {
            //make storage for open and closed nodes
            List<Node> openSet = new List<Node>();// - used for nonoptimised search
            //Heap<Node> openSet = new Heap<Node>(m_grid.m_maxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode); //add start node to open set

            while (openSet.Count > 0)//loop while there are open nodes
            {
                Node currentNode = NonOptimisedNodeSearch(openSet);//non Optimised version of node search

                //set current node to closed
                openSet.Remove(currentNode);// - used for nonoptimised search

                //optimised node search
                //Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    //debug timer text
                    sw.Stop();
                    print("Path found in:" + sw.ElapsedMilliseconds + "ms");

                    pathSucess = true;
                    break;
                }

                foreach (Node neighbour in m_grid.GetNeighbours(currentNode))
                {
                    //ignore node is not walkable or is closed
                    if (!neighbour.m_walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    //if this neighbour has a lower g cost, or is not open yet, move to it (set new g cost, and open it)
                    int newCostToNeighbour = currentNode.m_gCost + DistanceBetweenNodes(currentNode, neighbour);
                    if (newCostToNeighbour < neighbour.m_gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.m_gCost = newCostToNeighbour;
                        neighbour.m_hCost = DistanceBetweenNodes(neighbour, targetNode);
                        neighbour.m_parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }

        if(pathSucess)
        {
            waypoints = TracePath(startNode, targetNode);
            return waypoints;
        }

        return new Vector3[0];
    }

    Node NonOptimisedNodeSearch(List<Node> openSet)
    {
        Node currentNode = openSet[0];
        for (int i = 1; i < openSet.Count; i++)//search all other nodes in open set
        {
            switch (m_pathfindingType)
            {
                case EPathfindingType.AStar:
                    //set current node to open node with smallest f cost, or if f costs are equal, node with smallest h cost (not optimised)
                    if (openSet[i].m_fCost < currentNode.m_fCost || (openSet[i].m_fCost == currentNode.m_fCost && openSet[i].m_hCost < currentNode.m_hCost))
                    {
                        currentNode = openSet[i];
                    }
                    break;
                case EPathfindingType.BestFirst:
                    //best first search heuristics comparison
                    if (openSet[i].m_hCost < currentNode.m_hCost)
                    {
                        currentNode = openSet[i];
                    }
                    break;
                default:
                    break;
            }

        }

        return currentNode;
    }


    int DistanceBetweenNodes(Node node1, Node node2)
    {
        // if each perpendicular step is 1 distance, then each diagonal is root(2) distance
        //so to make things simpler, multiply by 10 to give nicer numbers to work with, so...
        //each perpendicular step is 10 distance, therfore each diagonal is 14 distance

        int distX = Mathf.Abs(node1.m_gridX - node2.m_gridX);
        int distY = Mathf.Abs(node1.m_gridY - node2.m_gridY);

        if(distX > distY)
        {
            return (14 * distY) + (10 * (distX - distY));
        }
        else
        {
            return (14 * distX) + (10 * (distY - distX));
        }
    }


    Vector3[] TracePath(Node startNode, Node endNode)
    {
        //start at the end node
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        //loop through parents until we find the start node
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.m_parent;
        }

        //path is in reverse, so we reverse to make it forwards
        path.Reverse();

        m_grid.m_path = path;

        //temp code to be improved later
        List<Vector3> waypoints = new List<Vector3>();
        foreach (Node node in path)
        {
            waypoints.Add(node.m_worldPos);
        }

        return waypoints.ToArray();
    }
}
