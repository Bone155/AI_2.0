using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Thief : MonoBehaviour
{
    public Agent agent;
    public List<Transform> guards;
    public GameObject panel;
    Decision root;
    // Start is called before the first frame update
    void Start()
    {
        panel.SetActive(false);
        root = new Detected(agent, guards,
                new Caught(agent, //yes
                    new Busted(agent), //yes
                    new Avoid(agent)), //no
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
            agent.caught = true;
        }
        if (other.gameObject.CompareTag("Exit"))
        {
            panel.SetActive(true);
            agent.navAgent.speed = 0;
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
            agent.chase = true;
        if (agent.chase)
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
        Ray[] rays = new Ray[7];
        for(int i = 0; i < guards.Count; i++)
        {
            int x = 0;
            for (int j = 0; j < rays.Length; j++)
            {
                rays[j] = new Ray(guards[i].position, new Vector3(guards[i].forward.x, guards[i].forward.y, guards[i].forward.z - (3 + x)));
                if (Physics.Raycast(rays[j], out RaycastHit hit, 3) && hit.transform.tag == "Thief")
                {
                    spotted = true;
                }
                else
                    x++;
                
            }
            
                
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
        if (agent.caught)
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
        agent.navAgent.speed = 0;
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
        agent.navAgent.speed += 0.05f;
        agent.caught = false;
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
    Agent agent;

    public ThiefExitPath() { }

    public ThiefExitPath(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.navAgent.SetDestination(GameObject.FindGameObjectWithTag("Exit").transform.position);
        return null;
    }
}

public class ThiefMove : Decision // answer node // seek a target
{
    Agent agent;

    public ThiefMove() { }

    public ThiefMove(Agent agent)
    {
        this.agent = agent;
    }

    public Decision makeDecision()
    {
        agent.navAgent.speed = 3.51f;
        agent.navAgent.SetDestination(GameObject.FindGameObjectWithTag("Diamond").transform.position);
        return null;
    }

}
