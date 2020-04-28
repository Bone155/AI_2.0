using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Agent : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public Vector3 velocity;
    public float mass;
    public float speed;
    float maxForce = 0.5f;
    Vector3 steeringForces;
    public Transform target;
    public List<Transform> waypoints;
    public int idx = 0;
    // Update is called once per frame
    void Update()
    {
        Vector3 steering = Vector3.ClampMagnitude(steeringForces, maxForce);
        steeringForces = Vector3.zero;
        steering /= mass;
        if (velocity != Vector3.zero)
        {
            transform.forward = velocity.normalized;
        }
        steeringForces += steering;

    }
}
