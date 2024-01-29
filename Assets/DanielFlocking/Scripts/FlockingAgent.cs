using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingAgent : MonoBehaviour
{
    [SerializeField] float m_speed;
    Vector3 m_velocity;
    [SerializeField] float m_awarenessRadius;
    int m_id = 0;
    // Start is called before the first frame update
    void Start()
    {
        m_velocity = new Vector3 (Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
        m_velocity.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += m_velocity * m_speed * Time.deltaTime;
    }

    public Vector3 GetVelocity() 
    {
        return m_velocity;
    }

    public void SetVelocity(Vector3 velocity) 
    {
        m_velocity = velocity;
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
