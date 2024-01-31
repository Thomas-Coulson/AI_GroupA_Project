using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingAgent : MonoBehaviour
{
    [SerializeField] float m_speed;
    Vector2 m_velocity;
    Vector3 m_translatedVelocity;
    [SerializeField] float m_awarenessRadius;
    int m_id = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_velocity = new Vector2 (Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        m_velocity.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        m_translatedVelocity.x = m_velocity.x;
        m_translatedVelocity.z = m_velocity.y;
        gameObject.transform.position += m_translatedVelocity * m_speed * Time.deltaTime;
        //gameObject.transform.rotation = Quaternion.LookRotation(gameObject.transform.position + m_translatedVelocity);
        transform.LookAt(gameObject.transform.position + m_translatedVelocity);
    }

    public Vector2 GetVelocity() 
    {
        return m_velocity;
    }

    public void SetVelocity(Vector2 velocity) 
    {
        m_velocity = velocity;
    }

    public Vector2 GetV2Position() 
    {
        return new Vector2(transform.position.x, transform.position.z);
    }

    public float GetAwarenessRadius() 
    { 
        return m_awarenessRadius;
    }

    public void SetAwarenessRadius(float awarenessRadius) 
    {
        m_awarenessRadius = awarenessRadius;
    }

    public int GetId() 
    {
        return m_id;
    }

    public void SetId(int id) 
    {
        m_id = id;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.7f);
        Gizmos.DrawSphere(transform.position, m_awarenessRadius);
    }
}
