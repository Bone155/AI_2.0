using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Move : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        agent.destination = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
