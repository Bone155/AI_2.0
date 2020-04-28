using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Decision
{
    Decision makeDecision();
}

public class AIGuard : MonoBehaviour
{
    public Agent agent;
    public List<Transform> waypoints;
    public Decision root;
    // Start is called before the first frame update
    void Start()
    {
        root = new Discovered(agent, 
                new thiefCaught(agent, //yes
                    new newWayPoint(agent), //no
                    new seekTarget(agent)), //yes
                new Waypoint(agent, //no
                    new newWayPoint(agent), //yes
                    new HavePath(agent, //no
                        new seekWayPoint(agent), //yes
                        new newWayPoint(agent)))); //no
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
        Ray ray = new Ray(agent.transform.position, agent.transform.forward);
        if ((agent.target.position.x > agent.transform.forward.x - 2 || agent.target.position.x < agent.transform.forward.x + 2) && Physics.Raycast(ray, out RaycastHit hit, 1))
            return true;
        else
            return false;
    }
}

public class thiefCaught : Decision // question node // caught him?
{
    Agent agent;
    Decision patrol;
    Decision seek;
    float sec = 0;
    public thiefCaught() { }

    public thiefCaught(Agent agent, Decision patrolDecision, Decision seekDecision)
    {
        this.agent = agent;
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

    bool thiefInView(Agent agent)
    {
        Ray ray = new Ray(agent.transform.position, agent.transform.forward);
        if ((agent.target.position.x > agent.transform.forward.x - 2 || agent.target.position.x < agent.transform.forward.x + 2) && Physics.Raycast(ray, out RaycastHit hit, 1))
            return true;
        else
            return false;
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
        agent.navAgent.destination = agent.target.position;
        return null;
    }

}

public class Waypoint : Decision //question node // reached waypoint?
{
    Agent agent;
    Decision closeEnough;
    Decision tooFar;

    public Waypoint() { }

    public Waypoint(Agent agent, Decision closeEnoughDecision, Decision tooFarDecision)
    {
        this.agent = agent;
        closeEnough = closeEnoughDecision;
        tooFar = tooFarDecision;
    }

    public Decision makeDecision()
    {
        if ()
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
    Agent agent;
    public seekWayPoint() { }

    public seekWayPoint(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        if (agent.idx >= agent.waypoints.Count)
            agent.idx = 0;
        agent.navAgent.destination = agent.waypoints[agent.idx].position;
        agent.idx++;
        return null;
    }
}

public class newWayPoint : Decision //answer node // get new waypoint
{
    Agent agent;

    public newWayPoint() { }

    public newWayPoint(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.path = agent.dj.calculatePath(agent.transform, agent.waypoints[agent.idx]);
        return null;
    }
}

public class HavePath : Decision // question node // does the agent have a path
{
    Agent agent;
    Decision yes;
    Decision no;

    public HavePath() { }

    public HavePath(Agent agent, Decision yesDecision, Decision noDecision)
    {
        this.agent = agent;
        yes = yesDecision;
        no = noDecision;
    }

    public Decision makeDecision()
    {
        if (agent.path.Count <= 0)
        {
            return no;
        }
        else
        {
            return yes;
        }

    }
}
