using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingAgent : MonoBehaviour
{
    [SerializeField] float m_speed;
    Vector2 m_velocity;
    Vector3 m_translatedVelocity;
    [SerializeField] float m_awarenessRadius;
    FlockingAgent m_leader = null;
    bool m_isLeader = false;
    int m_id = 0;
    [SerializeField] Material m_leaderMaterial;
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

    public void SetAwarenessRadius(float r) 
    {
        m_awarenessRadius = r;
    }

    public int GetId() 
    {
        return m_id;
    }

    public void SetId(int id) 
    {
        m_id = id;
    }

    public FlockingAgent GetLeader() 
    {
        return m_leader;
    }

    public void SetLeader(FlockingAgent l) 
    {
        m_leader = l;
    }

    public bool GetIsLeader() 
    {
        return m_isLeader;
    }

    public void SetIsLeader(bool l) 
    {
        m_isLeader = l;
        if (m_isLeader) 
        {
            Renderer r = gameObject.GetComponent<Renderer>();
            if (r != null) 
            {
                r.material = m_leaderMaterial;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.7f);
        Gizmos.DrawSphere(transform.position, m_awarenessRadius);
    }
}
