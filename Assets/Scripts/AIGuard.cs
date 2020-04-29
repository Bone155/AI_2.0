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
    public Transform target;
    public Decision root;
    public GameObject panel;
    bool atWaypoint = false;
    int idx = 0;
    // Start is called before the first frame update
    void Start()
    {
        GameObject wayTarget = new GameObject();
        GameObject[] waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        float dst = Vector3.Distance(agent.transform.position, waypoints[0].transform.position);
        foreach (GameObject o in waypoints)
        {
            float tmp = Vector3.Distance(agent.transform.position, o.transform.position);
            if (tmp < dst)
            {
                dst = tmp;
                wayTarget = o;
            }
        }
        agent.SetDestination(wayTarget.transform.position);
        panel.SetActive(false);
        root = new Discovered(agent, target,
                new thiefCaught(agent, target, //yes
                    new nextWayPoint(agent, atWaypoint, idx), //no
                    new seekTarget(agent, target)), //yes
                new Waypoint(agent, atWaypoint, //no
                    new seekWayPoint(agent, idx), //no
                    new nextWayPoint(agent, atWaypoint, idx))); //yes
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
        if (other.gameObject.CompareTag("Waypoint"))
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
        if ((target.position.x > agent.transform.forward.x - 2 || target.position.x < agent.transform.forward.x + 2) && Physics.Raycast(ray, out RaycastHit hit, 1))
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
    float sec = 0;
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
        if (!thiefInView(agent))
            sec += Time.deltaTime;
        else
            sec = 0;
        if (sec < 5)
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
        if ((target.position.x > agent.transform.forward.x - 2 || target.position.x < agent.transform.forward.x + 2) && Physics.Raycast(ray, out RaycastHit hit, 1))
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
    int idx;

    public seekWayPoint() { }

    public seekWayPoint(NavMeshAgent agent, int idx)
    {
        this.agent = agent;
        this.idx = idx;
    }

    public Decision makeDecision()
    {
        agent.SetDestination(GameObject.FindGameObjectWithTag("Waypoint").transform.position);
        return null;
    }
}

public class nextWayPoint : Decision //answer node // get new waypoint
{
    NavMeshAgent agent;
    bool atWay;
    int idx;

    public nextWayPoint() { }

    public nextWayPoint(NavMeshAgent agent, bool atWay, int idx)
    {
        this.agent = agent;
        this.atWay = atWay;
        this.idx = idx;
    }

    public Decision makeDecision()
    {
        if (idx >= GameObject.FindGameObjectsWithTag("Waypoint").Length)
            idx = 0;
        atWay = false;
        idx++;
        return null;
    }
}
