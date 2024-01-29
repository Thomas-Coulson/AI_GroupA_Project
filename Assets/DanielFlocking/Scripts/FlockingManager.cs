using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    List<FlockingAgent> m_agentsList;
    List<FlockingAgent> m_nearbyAgents;
    // Start is called before the first frame update
    void Start()
    {
        m_nearbyAgents = new List<FlockingAgent>();
        m_agentsList = new List<FlockingAgent>();
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
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_agentsList.Count; i++) 
        {
            if (m_agentsList[i] != null) 
            {
                Vector3 targetPos = m_agentsList[i].transform.position;
                for (int j = 0; j < m_agentsList.Count; j++) 
                {
                    if (m_agentsList[i].GetId() != m_agentsList[j].GetId()) 
                    {
                        Vector3 otherPos = m_agentsList[j].transform.position;
                        Vector3 diff = otherPos - targetPos;
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

            m_nearbyAgents.Clear();
        }
    }

    void Allignment(FlockingAgent agent) 
    {
        Vector3 velocitySum = Vector3.zero;
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
        Vector3 positionSum = Vector3.zero;
        for (int i = 0; i < m_nearbyAgents.Count; ++i)
        {
            positionSum += m_nearbyAgents[i].GetVelocity();
        }
        positionSum /= m_nearbyAgents.Count;
        Vector3 normal = positionSum - agent.transform.position;
        normal.Normalize();
        Vector3 vel = agent.GetVelocity();
        float velMag = vel.magnitude;
        vel.Normalize();
        vel = Vector3.Lerp(vel, normal, 0.1f);
        agent.SetVelocity(vel * velMag);
    }

    void Separation(FlockingAgent agent) 
    {
        Vector3 positionSum = Vector3.zero;
        for (int i = 0; i < m_nearbyAgents.Count; ++i)
        {
            positionSum += m_nearbyAgents[i].GetVelocity();
        }
        positionSum /= m_nearbyAgents.Count;
        Vector3 normal = positionSum - agent.transform.position;
        normal.Normalize();
        normal *= -1.0f;
        Vector3 vel = agent.GetVelocity();
        float velMag = vel.magnitude;
        vel.Normalize();
        vel = Vector3.Lerp(vel, normal, 0.1f);
        agent.SetVelocity(vel * velMag);
    }
}
