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
    public NavMeshAgent agent;
    public GameObject[] waypoints;
    public Transform target;
    public Decision root;
    public GameObject panel;
    bool atWaypoint = false;
    int idx = 0;
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
        root = new Discovered(agent, target,
                new thiefCaught(agent, target, //yes
                    new closestWaypoint(agent, waypoints, idx), //no
                    new seekTarget(agent, target)), //yes
                new Waypoint(agent, atWaypoint, //no
                    new seekWayPoint(agent, waypoints, idx), //no
                    new nextWayPoint(agent, waypoints, atWaypoint, idx))); //yes
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
        if (other.gameObject.CompareTag("Thief") && other.gameObject != null)
        {
            Destroy(other.gameObject);
            panel.SetActive(true);
        }
        if (other.gameObject == waypoints[idx])
        {
            atWaypoint = true;
        }
    }
}

public class Discovered : Decision // question node // discovered thief
{
    NavMeshAgent agent;
    Transform target;
    Decision discovered;
    Decision notDiscovered;

    public Discovered() { }

    public Discovered(NavMeshAgent agent, Transform target, Decision discoveredDecision, Decision notdiscoveredDecision)
    {
        this.agent = agent;
        this.target = target;
        discovered = discoveredDecision;
        notDiscovered = notdiscoveredDecision;
    }

    public Decision makeDecision()
    {
        if (thiefInView(agent))
        {
            return discovered;
        }
        else
        {
            return notDiscovered;
        }
    }

    bool thiefInView(NavMeshAgent agent)
    {
        Ray ray = new Ray(agent.transform.position, agent.transform.forward);
        if ((target.position.x > agent.transform.forward.x - 3 || target.position.x < agent.transform.forward.x + 3) && Physics.Raycast(ray, out RaycastHit hit, 4))
            return true;
        else
            return false;
    }
}

public class thiefCaught : Decision // question node // caught him?
{
    NavMeshAgent agent;
    Transform target;
    Decision patrol;
    Decision seek;
    public thiefCaught() { }

    public thiefCaught(NavMeshAgent agent, Transform target, Decision patrolDecision, Decision seekDecision)
    {
        this.agent = agent;
        this.target = target;
        patrol = patrolDecision;
        seek = seekDecision;
    }

    public Decision makeDecision()
    {
        bool thief = false;
        if (thiefInView(agent))
        {
            thief = true;
        }
        if (thief)
        {
            return seek;
        }
        else
        {
            return patrol;
        }

    }

    bool thiefInView(NavMeshAgent agent)
    {
        Ray ray = new Ray(agent.transform.position, agent.transform.forward);
        if ((target.position.x > agent.transform.forward.x - 3 || target.position.x < agent.transform.forward.x + 3) && Physics.Raycast(ray, out RaycastHit hit, 4))
            return true;
        else
            return false;
    }
}

public class seekTarget : Decision // answer node // seeking theif
{
    NavMeshAgent agent;
    Transform target;

    public seekTarget() { }

    public seekTarget(NavMeshAgent agent, Transform target)
    {
        this.agent = agent;
        this.target = target;
    }

    public Decision makeDecision()
    {
        agent.SetDestination(target.position);
        return null;
    }

}

public class Waypoint : Decision //question node // reached waypoint?
{
    NavMeshAgent agent;
    bool atWay;
    Decision closeEnough;
    Decision tooFar;

    public Waypoint() { }

    public Waypoint(NavMeshAgent agent, bool atWay, Decision tooFarDecision, Decision closeEnoughDecision)
    {
        this.agent = agent;
        this.atWay = atWay;
        closeEnough = closeEnoughDecision;
        tooFar = tooFarDecision;
    }

    public Decision makeDecision()
    {
        if (atWay)
        {
            return closeEnough;
        }
        else
        {
            return tooFar;
        }

    }

}

public class seekWayPoint : Decision //answer node // move towards waypoint
{
    NavMeshAgent agent;
    GameObject[] waypoints;
    int idx;

    public seekWayPoint() { }

    public seekWayPoint(NavMeshAgent agent, GameObject[] waypoints, int idx)
    {
        this.agent = agent;
        this.waypoints = waypoints;
        this.idx = idx;
    }

    public Decision makeDecision()
    {
        agent.SetDestination(waypoints[idx].transform.position);
        return null;
    }
}

public class nextWayPoint : Decision //answer node // get new waypoint
{
    NavMeshAgent agent;
    GameObject[] waypoints;
    bool atWay;
    int idx;

    public nextWayPoint() { }

    public nextWayPoint(NavMeshAgent agent, GameObject[] waypoints, bool atWay, int idx)
    {
        this.agent = agent;
        this.atWay = atWay;
        this.idx = idx;
        this.waypoints = waypoints;
    }

    public Decision makeDecision()
    {
        if (idx >= waypoints.Length)
            idx = 0;
        atWay = false;
        idx++;
        return null;
    }
}

public class closestWaypoint : Decision //answer node // get closest waypoint
{
    NavMeshAgent agent;
    GameObject[] waypoints;
    int idx;

    public closestWaypoint() { }

    public closestWaypoint(NavMeshAgent agent, GameObject[] waypoints, int idx)
    {
        this.agent = agent;
        this.idx = idx;
        this.waypoints = waypoints;
    }

    public Decision makeDecision()
    {
        GameObject wayTarget = new GameObject();
        float dst = Vector3.Distance(agent.transform.position, waypoints[0].transform.position);
        int count = 0;
        foreach (GameObject o in waypoints)
        {
            float tmp = Vector3.Distance(agent.transform.position, o.transform.position);
            if (tmp < dst)
            {
                wayTarget = o;
                idx = count;
            }
            count++;
        }
        agent.SetDestination(wayTarget.transform.position);
        return null;
    }
}