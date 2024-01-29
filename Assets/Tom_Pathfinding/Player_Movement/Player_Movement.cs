using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{

    List<Node> m_path;
    Vector3 m_targetPos;
    Node m_targetNode;

    Camera m_camera;

    void Awake()
    {
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

            //request AStar Path (maybe return path to follow)
            //if(AStar_Requests.RequestPath(transform.position, m_targetPos) != null)
            //{
            //    m_path = AStar_Requests.RequestPath(transform.position, m_targetPos);
            //    Debug.Log("Found valid path to destination");
            //}
            
            //do a null check for if the path was successfull (Doesnt like null, find another way around it) 11;52

            //Debug.Log("New target pos = " + m_targetPos);
        }
    }


}
