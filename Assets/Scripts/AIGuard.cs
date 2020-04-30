using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface Decision
{
    Decision makeDecision();
}

public class AIGuard : MonoBehaviour
{
    public Agent agent;
    public Decision root;
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        float dst = Vector3.Distance(agent.transform.position, agent.waypoints[agent.idx].transform.position);
        for (int i = 0; i < agent.waypoints.Length; i++)
        {
            float tmp = Vector3.Distance(agent.transform.position, agent.waypoints[i].transform.position);
            if (tmp < dst)
            {
                agent.idx = i;
            }
        }
        panel.SetActive(false);
        root = new Discovered(agent,
                new thiefCaught(agent, //yes
                    new closestWaypoint(agent), //no
                    new seekTarget(agent)), //yes
                new Waypoint(agent, //no
                    new seekWayPoint(agent), //no
                    new nextWayPoint(agent))); //yes
    }

    // Update is called once per frame
    void Update()
    {
        Decision currentDecision = root;
        while (currentDecision != null)
        {
            currentDecision = currentDecision.makeDecision();
        }

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Thief"))
        {
            Destroy(other.gameObject);
            panel.SetActive(true);
        }
        if (other.gameObject == agent.waypoints[agent.idx])
        {
            agent.atWaypoint = true;
        }
    }
}

public class Discovered : Decision // question node // discovered thief
{
    Agent agent;
    Decision discovered;
    Decision notDiscovered;
    public Discovered() { }

    public Discovered(Agent agent, Decision discoveredDecision, Decision notdiscoveredDecision)
    {
        this.agent = agent;
        discovered = discoveredDecision;
        notDiscovered = notdiscoveredDecision;
    }

    public Decision makeDecision()
    {
        if (thiefInView(agent))
            agent.chase = true;
        if (agent.chase)
        {
            return discovered;
        }
        else
        {
            return notDiscovered;
        }
    }

    bool thiefInView(Agent agent)
    {
        Ray ray;
        int x = 0;
        bool line = false;
        for(int i = 0; i < 7; i++)
        {
            ray = new Ray(agent.transform.position, new Vector3(agent.transform.forward.x, agent.transform.forward.y, agent.transform.forward.z + (3 - x)));
            if (Physics.Raycast(ray, out RaycastHit hit, 10) && hit.transform.tag == "Thief")
            {
                Debug.DrawLine(ray.origin, agent.target.transform.position, Color.cyan);
                line = true;
            }
            else
                x++;
        }
        return line;
    }
}

public class thiefCaught : Decision // question node // caught him?
{
    Agent agent;
    Decision patrol;
    Decision seek;
    public thiefCaught() { }

    public thiefCaught(Agent agent, Decision patrolDecision, Decision seekDecision)
    {
        this.agent = agent;
        patrol = patrolDecision;
        seek = seekDecision;
    }

    public Decision makeDecision()
    {
        if (agent.chase)
        {
            return seek;
        }
        else
        {
            return patrol;
        }

    }

    
}

public class seekTarget : Decision // answer node // seeking theif
{
    Agent agent;

    public seekTarget() { }

    public seekTarget(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.navAgent.SetDestination(agent.target.transform.position);
        agent.navAgent.speed = 3.61f;
        return null;
    }

}

public class Waypoint : Decision //question node // reached waypoint?
{
    Agent agent;
    Decision closeEnough;
    Decision tooFar;

    public Waypoint() { }

    public Waypoint(Agent agent, Decision tooFarDecision, Decision closeEnoughDecision)
    {
        this.agent = agent;
        closeEnough = closeEnoughDecision;
        tooFar = tooFarDecision;
    }

    public Decision makeDecision()
    {
        if (agent.atWaypoint)
        {
            return closeEnough;
        }
        else
            return tooFar;

    }

}

public class seekWayPoint : Decision //answer node // move towards waypoint
{
    Agent agent;

    public seekWayPoint() { }

    public seekWayPoint(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.navAgent.speed = 3.21f;
        agent.navAgent.SetDestination(agent.waypoints[agent.idx].transform.position);
        return null;
    }
}

public class nextWayPoint : Decision //answer node // get new waypoint
{
    Agent agent;

    public nextWayPoint() { }

    public nextWayPoint(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.idx++;
        if (agent.idx >= agent.waypoints.Length)
            agent.idx = 0;
        agent.atWaypoint = false;
        return null;
    }
}

public class closestWaypoint : Decision //answer node // get closest waypoint
{
    Agent agent;

    public closestWaypoint() { }

    public closestWaypoint(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        GameObject wayTarget = new GameObject();
        float dst = Vector3.Distance(agent.transform.position, agent.waypoints[0].transform.position);
        int count = 0;
        foreach (GameObject o in agent.waypoints)
        {
            float tmp = Vector3.Distance(agent.transform.position, o.transform.position);
            if (tmp < dst)
            {
                wayTarget = o;
                agent.idx = count;
            }
            count++;
        }
        agent.navAgent.SetDestination(wayTarget.transform.position);
        return null;
    }
}