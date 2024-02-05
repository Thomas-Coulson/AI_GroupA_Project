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
    FlockingManager m_manager;
    List<Vector2> m_possibleVelocities = new List<Vector2>();

    [SerializeField]
    public enum DecisionTypes
    {
        E_CHASE_LEADER,
        E_AVOID_OBSTACLE,
        E_IN_FLOCK
    }

    public Dictionary<DecisionTypes, float> m_weights = new Dictionary<DecisionTypes, float>();

    // Start is called before the first frame update
    void Start()
    {
        m_velocity = new Vector2 (Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        m_velocity.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_manager != null) 
        {
            m_possibleVelocities.Add(m_manager.ChaseLeader(this));
            m_possibleVelocities.Add(m_manager.Avoidance(this));
            m_possibleVelocities.Add(m_manager.InFlock(this));

            m_possibleVelocities[(int)DecisionTypes.E_CHASE_LEADER] *= m_weights[DecisionTypes.E_CHASE_LEADER];
            m_possibleVelocities[(int)DecisionTypes.E_AVOID_OBSTACLE] *= m_weights[DecisionTypes.E_AVOID_OBSTACLE];
            m_possibleVelocities[(int)DecisionTypes.E_IN_FLOCK] *= m_weights[DecisionTypes.E_IN_FLOCK];

            Vector2 sum = Vector2.zero;
            int count = 0;
            foreach (Vector2 v in m_possibleVelocities)
            {
                if (v != Vector2.zero) 
                {
                    sum += v;
                    count++;
                }
            }

            if (count > 0) 
            {
                sum /= count;
                sum.Normalize();
                m_velocity = sum;
            }

            m_possibleVelocities.Clear();
        }
        else 
        {
            InitialiseWeightsAndRefs();
        }
        m_translatedVelocity.x = m_velocity.x;
        m_translatedVelocity.z = m_velocity.y;
        gameObject.transform.position += m_translatedVelocity * m_speed * Time.deltaTime;
        //gameObject.transform.rotation = Quaternion.LookRotation(gameObject.transform.position + m_translatedVelocity);
        transform.LookAt(gameObject.transform.position + m_translatedVelocity);
    }

    public void InitialiseWeightsAndRefs() 
    {
        if (m_isLeader || m_leader == null)
            m_weights.Add(DecisionTypes.E_CHASE_LEADER, 0.0f);
        else
            m_weights.Add(DecisionTypes.E_CHASE_LEADER, 10.0f);
        m_weights.Add(DecisionTypes.E_AVOID_OBSTACLE, 5.0f);
        m_weights.Add(DecisionTypes.E_IN_FLOCK, 1.0f);

        m_manager = FindObjectOfType<FlockingManager>();
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
