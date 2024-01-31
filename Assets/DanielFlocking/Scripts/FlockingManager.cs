using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    List<FlockingAgent> m_agentsList;
    List<FlockingAgent> m_nearbyAgents;
    List<GameObject> m_obstacles;

    [SerializeField] float m_obstacleAvoidanceWeight = 1.0f;
    [SerializeField] float m_restObstacleAvoidanceMagnitude = 0.75f;
    // Start is called before the first frame update
    void Start()
    {
        m_nearbyAgents = new List<FlockingAgent>();
        m_agentsList = new List<FlockingAgent>();
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
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_agentsList.Count; i++) 
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

            m_nearbyAgents.Clear();
        }
    }

    void Allignment(FlockingAgent agent) 
    {
        Vector2 velocitySum = Vector2.zero;
        for (int i = 0; i < m_nearbyAgents.Count; ++i) 
        {
            velocitySum += m_nearbyAgents[i].GetVelocity();
        }

        velocitySum /= m_nearbyAgents.Count;
        velocitySum.Normalize();
        agent.SetVelocity(velocitySum);
    }

    void Cohesion(FlockingAgent agent) 
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
        Vector2 vel = agent.GetVelocity();
        float velMag = vel.magnitude;
        vel.Normalize();
        vel = Vector2.Lerp(vel, normal, 0.1f);
        agent.SetVelocity(vel * velMag);
    }

    void Separation(FlockingAgent agent) 
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
        Vector2 vel = agent.GetVelocity();
        float velMag = vel.magnitude;
        vel.Normalize();
        vel = Vector2.Lerp(vel, normal, 0.1f);
        agent.SetVelocity(vel * velMag);
    }

    void Avoidance(FlockingAgent agent) 
    {
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
            average *= m_obstacleAvoidanceWeight;
            Vector2 newVel = agent.GetVelocity() + average;
            newVel.Normalize();
            agent.SetVelocity(newVel);
        }
    }
}
