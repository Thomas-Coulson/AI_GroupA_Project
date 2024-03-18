using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingAgent : MonoBehaviour
{
    [SerializeField] float m_speed;
    [SerializeField] float m_chaseSpeed;
    [SerializeField] float m_flockSpeed;
    [SerializeField] float m_speedRateOfChange;

    Vector2 m_velocity;
    Vector3 m_translatedVelocity;
    [SerializeField] float m_awarenessRadius;
    FlockingAgent m_leader = null;
    bool m_isLeader = false;
    int m_id = 0;
    [SerializeField] Material m_leaderMaterial;
    FlockingManager m_manager;
    [SerializeField] List<Vector2> m_possibleVelocities = new List<Vector2>();
    Vector2 m_wanderTarget = new Vector2(9999999999.0f, 9999999999.0f);
    int m_flockId = -1;

    [SerializeField]
    public enum DecisionTypes
    {
        E_CHASE_LEADER,
        E_LEADER_WANDER,
        E_CLEAR_LEADER_PATH,
        E_AVOID_OBSTACLE,
        E_IN_FLOCK,
        E_NUMBER_OF_DECISION_TYPES
    }

    public Dictionary<DecisionTypes, float> m_weights = new Dictionary<DecisionTypes, float>();

    // Start is called before the first frame update
    void Start()
    {
        m_velocity = new Vector2 (Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
        m_velocity.Normalize();

        Vector2 pos = new Vector2(Random.Range(-80.0f, 80.0f), Random.Range(-80.0f, 80.0f));
        transform.position = new Vector3(pos.x, 0.0f, pos.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_manager != null) 
        {
            if (m_weights.Count < (int)DecisionTypes.E_NUMBER_OF_DECISION_TYPES)
                InitialiseWeightsAndRefs();

            m_possibleVelocities.Clear();

            m_possibleVelocities.Add(m_manager.ChaseLeader(this));
            m_possibleVelocities.Add(m_manager.Wander(this));
            m_possibleVelocities.Add(m_manager.EvadeLeader(this));
            m_possibleVelocities.Add(m_manager.Avoidance(this));
            m_possibleVelocities.Add(m_manager.InFlock(this));

            m_possibleVelocities[(int)DecisionTypes.E_CHASE_LEADER] *= m_weights[DecisionTypes.E_CHASE_LEADER];
            m_possibleVelocities[(int)DecisionTypes.E_LEADER_WANDER] *= m_weights[DecisionTypes.E_LEADER_WANDER];
            m_possibleVelocities[(int)DecisionTypes.E_CLEAR_LEADER_PATH] *= m_weights[DecisionTypes.E_CLEAR_LEADER_PATH];
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
        }
        else 
        {
            InitialiseWeightsAndRefs();
        }

        if (m_leader != null) 
        {
            if (m_leader.GetFlockId() == m_flockId) 
            {
                m_speed -= m_speedRateOfChange * Time.deltaTime;

                if (m_speed < m_flockSpeed) 
                {
                    m_speed = m_flockSpeed;
                }
            }
            else 
            {
                m_speed += m_speedRateOfChange * Time.deltaTime;

                if (m_speed > m_chaseSpeed) 
                {
                    m_speed = m_chaseSpeed;
                }
            }
        }
        else 
        {
            m_speed = m_flockSpeed;
        }


        m_translatedVelocity.x = m_velocity.x;
        m_translatedVelocity.z = m_velocity.y;
        gameObject.transform.position += m_translatedVelocity * m_speed * Time.deltaTime;
        //gameObject.transform.rotation = Quaternion.LookRotation(gameObject.transform.position + m_translatedVelocity);
        transform.LookAt(gameObject.transform.position + m_translatedVelocity);

        if (m_leader != null)
        {
            if (m_flockId == m_leader.GetFlockId())
            {
                if (m_weights[DecisionTypes.E_CHASE_LEADER] > 0.0f) 
                {
                    m_weights[DecisionTypes.E_CHASE_LEADER] -= Time.deltaTime * 50.0f;
                    if (m_weights[DecisionTypes.E_CHASE_LEADER] < 0.0f)
                        m_weights[DecisionTypes.E_CHASE_LEADER] = 0.0f;
                }
                //m_weights[DecisionTypes.E_CHASE_LEADER] = 0.0f;
            }
            else
            {
                m_weights[DecisionTypes.E_CHASE_LEADER] = 100.0f;
            }
        }
        else
        {
            m_weights[DecisionTypes.E_CHASE_LEADER] = 0.0f;
        }
    }

    public void InitialiseWeightsAndRefs() 
    {
        if (m_isLeader || m_leader == null) 
        {
            m_weights.Add(DecisionTypes.E_CHASE_LEADER, 0.0f);
            m_weights.Add(DecisionTypes.E_LEADER_WANDER, 10.0f);
            m_weights.Add(DecisionTypes.E_CLEAR_LEADER_PATH, 0.0f);
        }
        else
        {
            m_weights.Add(DecisionTypes.E_CHASE_LEADER, 100.0f);
            m_weights.Add(DecisionTypes.E_LEADER_WANDER, 0.0f);
            m_weights.Add(DecisionTypes.E_CLEAR_LEADER_PATH, 10.0f);
        }

        m_weights.Add(DecisionTypes.E_AVOID_OBSTACLE, 200.0f);

        if (m_isLeader)
            m_weights.Add(DecisionTypes.E_IN_FLOCK, 0.0f);
        else
            m_weights.Add(DecisionTypes.E_IN_FLOCK, 100.0f);

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

    public Vector2 GetWanderTarget() 
    {
        return m_wanderTarget;
    }

    public void SetWanderTarget(Vector2 w) 
    {
        m_wanderTarget = w;
    }

    public int GetFlockId() 
    {
        return m_flockId;
    }

    public void SetFlockId(int id) 
    {
        m_flockId = id;
    }

    private void OnDrawGizmos()
    {
        if (m_manager != null) 
        {
            Gizmos.color = m_manager.m_colorList[m_flockId];
            Gizmos.DrawSphere(transform.position, m_awarenessRadius);
        }
        else 
        {
            Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.25f);
            Gizmos.DrawSphere(transform.position, m_awarenessRadius);
        }

        if (m_isLeader) 
        {
            Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.25f);
            Gizmos.DrawSphere(new Vector3(m_wanderTarget.x, 0.0f, m_wanderTarget.y), 5.0f);
        }
    }
}
