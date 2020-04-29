using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public GameObject[] waypoints;
    public GameObject target;
    public bool atWaypoint = false;
    public bool caught = false;
    public int idx = 0;
}
