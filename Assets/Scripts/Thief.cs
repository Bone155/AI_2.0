using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Thief : MonoBehaviour
{
    public NavMeshAgent agent;
    public List<Transform> guards;
    public List<Transform> targets;
    public GameObject panel;
    Decision root;
    bool caught = false;
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
        root = new Detected(agent, guards,
                new Caught(agent, caught, //yes
                    new Busted(agent), //yes
                    new Avoid(agent, caught)), //no
                new ThiefMovement( //no
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

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Diamond"))
        {
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Security Guard"))
        {
            caught = true;
        }
        if (other.gameObject.CompareTag("Exit"))
        {
            panel.SetActive(true);
            agent.speed = 0;
        }
    }

}

public class Detected : Decision // question node // have been detected?
{
    NavMeshAgent agent;
    List<Transform> guards;
    Decision detected;
    Decision notDetected;

    public Detected() { }

    public Detected(NavMeshAgent agent, List<Transform> guards, Decision detectedDecision, Decision notDetectedDecision)
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

    bool inView(NavMeshAgent agent, List<Transform> guards)
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
    NavMeshAgent agent;
    bool caught;
    Decision busted;
    Decision avoided;

    public Caught() { }

    public Caught(NavMeshAgent agent, bool caught, Decision bustedDecision, Decision avoidedDecision)
    {
        this.agent = agent;
        this.caught = caught;
        busted = bustedDecision;
        avoided = avoidedDecision;
    }

    public Decision makeDecision()
    {
        if (caught)
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
    NavMeshAgent agent;

    public Busted() { }

    public Busted(NavMeshAgent agent)
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
    NavMeshAgent agent;
    bool caught;

    public Avoid() { }

    public Avoid(NavMeshAgent agent, bool caught)
    {
        this.agent = agent;
        this.caught = caught;
    }

    public Decision makeDecision()
    {
        agent.speed *= 2;
        caught = false;
        return null;
    }
}

public class ThiefMovement : Decision // question node // heist complete?
{
    Decision targetsStolen;
    Decision notStolenYet;

    public ThiefMovement() { }

    public ThiefMovement(Decision targetsDecision, Decision notStolenYetDecision)
    {
        targetsStolen = targetsDecision;
        notStolenYet = notStolenYetDecision;
    }

    public Decision makeDecision()
    {
        if (GameObject.FindGameObjectsWithTag("Diamond").Length <= 0)
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
    NavMeshAgent agent;

    public ThiefExitPath() { }

    public ThiefExitPath(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.SetDestination(GameObject.FindGameObjectWithTag("Exit").transform.position);
        return null;
    }
}

public class ThiefMove : Decision // answer node // seek a target
{
    NavMeshAgent agent;

    public ThiefMove() { }

    public ThiefMove(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.speed = 2.24f;
        agent.SetDestination(GameObject.FindGameObjectWithTag("Diamond").transform.position);
        return null;
    }

}
