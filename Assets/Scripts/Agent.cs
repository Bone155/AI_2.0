using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public GameObject[] waypoints;
    public GameObject target;
    public bool atWaypoint = false;
    public bool caught = false;
    public bool chase = false;
    public int idx = 0;
}
