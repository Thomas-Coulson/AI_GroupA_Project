using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class FlockingManager : MonoBehaviour
{
    List<FlockingAgent> m_agentsList;
    List<FlockingAgent> m_nearbyAgents;
    List<FlockingAgent> m_leaderAgents;
    List<GameObject> m_obstacles;

    [SerializeField] float m_obstacleAvoidanceWeight = 1.0f;
    [SerializeField] float m_restObstacleAvoidanceMagnitude = 0.75f;
    [SerializeField] float m_chaseLeaderWeight = 1.0f;
    [SerializeField] int m_numberOfLeaders = 3;
    // Start is called before the first frame update
    void Start()
    {
        m_nearbyAgents = new List<FlockingAgent>();
        m_agentsList = new List<FlockingAgent>();
        m_leaderAgents = new List<FlockingAgent>();
        m_obstacles = new List<GameObject>();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Agent");
        int count = 0;
        foreach (GameObject obj in objs) 
        {
            if (obj != null) 
            {
                FlockingAgent agent = obj.gameObject.GetComponent<FlockingAgent>();
                if (agent != null) 
                {
                    agent.SetId(count);
                    m_agentsList.Add(agent);
                    count++;
                }
            }
        }

        GameObject[] obs = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject ob in obs) 
        {
            if (ob != null) 
            {
                m_obstacles.Add(ob);
            }
        }

        if (m_numberOfLeaders <= 0)
            m_numberOfLeaders = 1;

        for (int i = 0; i < m_numberOfLeaders; i++) 
        {
            int rng = Random.Range(0, m_agentsList.Count);

            if (m_agentsList[rng] == null) 
            {
                i--;
                continue;
            }
            else if (m_agentsList[rng].GetIsLeader()) 
            {
                i--;
                continue;
            }

            m_agentsList[rng].SetIsLeader(true);
            m_leaderAgents.Add(m_agentsList[rng]);
        }

        foreach (FlockingAgent agent in m_agentsList) 
        {
            if (!agent.GetIsLeader()) 
            {
                int rng = Random.Range(0, m_leaderAgents.Count);
                agent.SetLeader(m_agentsList[rng]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*for (int i = 0; i < m_agentsList.Count; i++) 
        {
            if (m_agentsList[i] != null) 
            {
                Vector2 targetPos = m_agentsList[i].GetV2Position();
                for (int j = 0; j < m_agentsList.Count; j++) 
                {
                    if (m_agentsList[i].GetId() != m_agentsList[j].GetId()) 
                    {
                        Vector2 otherPos = m_agentsList[j].GetV2Position();
                        Vector2 diff = otherPos - targetPos;
                        if (diff.magnitude <= m_agentsList[i].GetAwarenessRadius()) 
                        {
                            m_nearbyAgents.Add(m_agentsList[j]);
                        }
                    } 
                }
            }

            if (m_nearbyAgents.Count > 0) 
            {
                Allignment(m_agentsList[i]);
                Cohesion(m_agentsList[i]);
                Separation(m_agentsList[i]);
            }

            if (m_obstacles.Count > 0)
                Avoidance(m_agentsList[i]);

            if (!m_agentsList[i].GetIsLeader()) 
            {
                ChaseLeader(m_agentsList[i]);
            }

            m_nearbyAgents.Clear();
        }*/
    }

    public Vector2 InFlock(FlockingAgent agent) 
    {
        Vector2 newVel = agent.GetVelocity();

        if (agent != null)
        {
            Vector2 targetPos = agent.GetV2Position();
            for (int i = 0; i < m_agentsList.Count; i++)
            {
                if (agent.GetId() != m_agentsList[i].GetId())
                {
                    Vector2 otherPos = m_agentsList[i].GetV2Position();
                    Vector2 diff = otherPos - targetPos;
                    if (diff.magnitude <= agent.GetAwarenessRadius())
                    {
                        m_nearbyAgents.Add(m_agentsList[i]);
                    }
                }
            }
        }

        if (m_nearbyAgents.Count > 0)
        {
            newVel = Allignment(agent, newVel);
            newVel = Cohesion(agent, newVel);
            newVel = Separation(agent, newVel);
            agent.m_weights[FlockingAgent.DecisionTypes.E_IN_FLOCK] = 1.0f;
        }
        else 
        {
            agent.m_weights[FlockingAgent.DecisionTypes.E_IN_FLOCK] = 0.0f;
        }

        m_nearbyAgents.Clear();
        return newVel;
    }

    Vector2 Allignment(FlockingAgent agent, Vector2 vel) 
    {
        Vector2 velocitySum = Vector2.zero;
        for (int i = 0; i < m_nearbyAgents.Count; ++i) 
        {
            velocitySum += m_nearbyAgents[i].GetVelocity();
        }

        velocitySum /= m_nearbyAgents.Count;
        velocitySum.Normalize();
        vel = velocitySum;
        return vel;
    }

    Vector2 Cohesion(FlockingAgent agent, Vector2 vel) 
    {
        Vector2 positionSum = Vector2.zero;
        Vector2 agentPos = agent.GetV2Position();
        for (int i = 0; i < m_nearbyAgents.Count; ++i)
        {
            positionSum += m_nearbyAgents[i].GetVelocity();
        }
        positionSum /= m_nearbyAgents.Count;
        Vector2 normal = positionSum - agentPos;
        normal.Normalize();
        Vector2 velocity = vel;
        float velMag = vel.magnitude;
        velocity.Normalize();
        velocity = Vector2.Lerp(velocity, normal, 0.1f);
        vel = velocity * velMag;
        return vel;
    }

    Vector2 Separation(FlockingAgent agent, Vector2 vel) 
    {
        Vector2 positionSum = Vector2.zero;
        Vector2 agentPos = agent.GetV2Position();
        for (int i = 0; i < m_nearbyAgents.Count; ++i)
        {
            positionSum += m_nearbyAgents[i].GetVelocity();
        }
        positionSum /= m_nearbyAgents.Count;
        Vector2 normal = positionSum - agentPos;
        normal.Normalize();
        normal *= -1.0f;
        Vector2 velocity = vel;
        float velMag = velocity.magnitude;
        velocity.Normalize();
        velocity = Vector2.Lerp(velocity, normal, 0.1f);
        vel = velocity * velMag;
        return vel;
    }

    public Vector2 Avoidance(FlockingAgent agent) 
    {
        if (m_obstacles.Count <= 0) 
        {
            agent.m_weights[FlockingAgent.DecisionTypes.E_AVOID_OBSTACLE] = 0.0f;
            return agent.GetVelocity();
        }
        Vector2 average = Vector2.zero;
        int count = 0;
        foreach (GameObject obstacle in m_obstacles) 
        {
            Vector2 v2ObsPos = Vector2.zero;
            v2ObsPos.x = obstacle.transform.position.x;
            v2ObsPos.y = obstacle.transform.position.z;

            Vector2 diff = agent.GetV2Position() - v2ObsPos;
            if (diff.magnitude <= agent.GetAwarenessRadius()) 
            {
                float mag = diff.magnitude;
                if (mag < (agent.GetAwarenessRadius() * m_restObstacleAvoidanceMagnitude)) 
                {
                    mag += (mag - m_restObstacleAvoidanceMagnitude);
                    diff.Normalize();
                    diff *= mag;
                }
                else if (mag > (agent.GetAwarenessRadius() * m_restObstacleAvoidanceMagnitude)) 
                {
                    mag -= (mag - (agent.GetAwarenessRadius() * m_restObstacleAvoidanceMagnitude));

                    if (mag <= 0.0f)
                        continue;

                    diff.Normalize();
                    diff *= mag;
                }
                count++;
                average += diff;
            }
        }

        if (count > 0) 
        {
            average /= count;
            average.Normalize();
            //average *= m_obstacleAvoidanceWeight;
            //Vector2 newVel = agent.GetVelocity() + average;
            //newVel.Normalize();
            agent.m_weights[FlockingAgent.DecisionTypes.E_AVOID_OBSTACLE] = 1.0f;
            return average;
        }

        agent.m_weights[FlockingAgent.DecisionTypes.E_AVOID_OBSTACLE] = 0.0f;
        return agent.GetVelocity();
    }

    public Vector2 ChaseLeader(FlockingAgent agent) 
    {
        if (agent.GetIsLeader()) 
        {
            return agent.GetVelocity();
        }

        FlockingAgent leader = agent.GetLeader();
        Vector2 leaderPos = leader.GetV2Position();
        Vector2 diff = leaderPos - agent.GetV2Position();
        diff.Normalize();
        Vector2 vel = agent.GetVelocity();
        float velMag = vel.magnitude;
        vel.Normalize();
        vel = Vector2.Lerp(vel, diff, 0.1f);
        vel.Normalize();
        return vel * velMag;
    }
}
