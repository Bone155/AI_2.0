using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thief : MonoBehaviour
{
    public Agent agent;
    public List<Transform> guards;
    public List<Transform> targets;
    Decision root;
    public int score;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        root = new Detected(agent, guards,
                new Caught(agent, //yes
                    new Busted(agent), //yes
                    new Avoid(agent)), //no
                new ThiefMovement(agent, targets, //no
                    new ThiefExitPath(agent), //yes
                    new ThiefMove(agent))); //no
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Diamond")
        {
            Destroy(other);
            score += 100;
        }
        
    }

}

public class Detected : Decision // question node // have been detected?
{
    Agent agent;
    List<Transform> guards;
    Decision detected;
    Decision notDetected;

    public Detected() { }

    public Detected(Agent agent, List<Transform> guards, Decision detectedDecision, Decision notDetectedDecision)
    {
        this.agent = agent;
        this.guards = guards;
        detected = detectedDecision;
        notDetected = notDetectedDecision;
    }

    public Decision makeDecision()
    {
        if (inView(agent, guards))
        {
            return detected;
        }
        else
        {
            return notDetected;
        }
    }

    bool inView(Agent agent, List<Transform> guards)
    {
        bool spotted = false;
        Ray ray;
        for(int i = 0; i < guards.Count; i++)
        {
            ray = new Ray(guards[i].position, guards[i].forward);
            if ((agent.transform.position.x > guards[i].forward.x - 2 || agent.transform.position.x < guards[i].forward.x + 2) && Physics.Raycast(ray, out RaycastHit hit, 1))
                spotted = true;
        }
        return spotted;
    }
}

public class Caught : Decision // question node // have been caught?
{
    Agent agent;
    Decision busted;
    Decision avoided;

    public Caught() { }

    public Caught(Agent agent, Decision bustedDecision, Decision avoidedDecision)
    {
        this.agent = agent;
        busted = bustedDecision;
        avoided = avoidedDecision;
    }

    public Decision makeDecision()
    {
        if ()
        {
            return busted;
        }
        else
        {
            return avoided;
        }

    }
}

public class Busted : Decision // answer node // got busted!
{
    Agent agent;

    public Busted() { }

    public Busted(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        //Game over
        agent.speed = 0;
        return null;
    }
}

public class Avoid : Decision // answer node // avoid guards
{
    Agent agent;

    public Avoid() { }

    public Avoid(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.navAgent.speed *= 2;
        return null;
    }
}

public class ThiefMovement : Decision // question node // stole all diamonds
{
    Agent agent;
    List<Transform> targets;
    Decision targetsStolen;
    Decision notStolenYet;

    public ThiefMovement() { }

    public ThiefMovement(Agent agent, List<Transform> targets, Decision targetsDecision, Decision notStolenYetDecision)
    {
        this.agent = agent;
        this.targets = targets;
        targetsStolen = targetsDecision;
        notStolenYet = notStolenYetDecision;
    }

    public Decision makeDecision()
    {
        if (targets.Count <= 0)
        {
            return targetsStolen;
        }
        else
        {
            return notStolenYet;
        }

    }
}

public class ThiefExitPath : Decision // answer node // set exit path
{
    Agent agent;

    public ThiefExitPath() { }

    public ThiefExitPath(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.navAgent.destination = GameObject.FindGameObjectWithTag("Exit").transform.position;
        return null;
    }
}

public class ThiefMove : Decision // answer node // seek target
{
    Agent agent;
    public ThiefMove() { }

    public ThiefMove(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.navAgent.destination = agent.target.position;
        return null;
    }

}
