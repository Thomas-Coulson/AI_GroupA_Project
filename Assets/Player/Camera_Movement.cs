using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class Camera_Movement : MonoBehaviour
{

    public float xSpeed;
    public float ySpeed;
    public float zoomSpeed;

    Vector3 pos;
    Vector3 lookDir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        calculateMovement();
    }

    void calculateMovement()
    {
        pos = transform.position;
        lookDir = transform.forward;

        pos.x += Input.GetAxis("Horizontal") * Time.deltaTime * xSpeed;
        pos.z += Input.GetAxis("Vertical") * Time.deltaTime * ySpeed;

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            pos += lookDir * Time.deltaTime * zoomSpeed;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            pos += lookDir * Time.deltaTime * -zoomSpeed;
        }

        transform.position = pos;

    }
}
