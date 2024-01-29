using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar_Requests : MonoBehaviour
{
    static AStar_Requests instance;
    AStarPathfinding m_aStar;

    private void Awake()
    {
        m_aStar = GetComponent<AStarPathfinding>();
    }

    //static function so it can be called from player
    public static List<Node> RequestPath(Vector3 startPos, Vector3 targetPos)
    {
        return instance.m_aStar.StartPathfinding(startPos, targetPos);
    }
}