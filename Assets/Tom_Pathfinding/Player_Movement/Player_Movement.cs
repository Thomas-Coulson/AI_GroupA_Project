using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{

    Vector3[] m_waypoints;
    Vector3 m_targetPos;
    int m_nextWaypointIndex;
    int m_moveLoopCount;

    public float m_speed;

    Camera m_camera;

    void Awake()
    {
        m_waypoints = new Vector3[0];
        m_camera = Camera.main;
    }

    private void Update()
    {
        //detect mouse click (to move player)
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            Ray ray = m_camera.ScreenPointToRay(mousePos);
            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                //hit variable shows what was clicked on
                m_targetPos = hit.point;
            }

            Debug.Log("Mouse click detected");

            //request AStar Path(maybe return path to follow)
            m_waypoints = AStar_Requests.RequestPath(transform.position, m_targetPos);
            if (m_waypoints.Length > 1)
            {
                Debug.Log("Waypoints length = " + m_waypoints.Length);

                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
                //FollowPath();

                Debug.Log("Movement loops = " + m_moveLoopCount);
            }
            else
            {
                Debug.Log("Invalid path");
            }

            //do a null check for if the path was successfull(Doesnt like null, find another way around it) 11; 52

            //Debug.Log("New target pos = " + m_targetPos);
        }
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = m_waypoints[0];
        m_nextWaypointIndex = 0;
        m_moveLoopCount = 0;

        while (true)
        {
            if(transform.position == currentWaypoint)
            {
                m_nextWaypointIndex++;

                //break if at the final target point
                if(m_nextWaypointIndex >= m_waypoints.Length)
                {
                    break;
                }

                currentWaypoint = m_waypoints[m_nextWaypointIndex];
            }

            //move towards the next target point
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, m_speed * Time.deltaTime);
            m_moveLoopCount++;

            yield return null;
        }



    }

}
