// UberDriverAI
// by Zomawia Sailo

// The AI will find passengers that need a ride to their destination.
// It will pick them up and make them a child object.
// Then it will drive them to their destination.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UberDriverAI : MonoBehaviour
{
    // area mask bit codes
    const int INTERSECTION = 8;
    const int I = 520;          //intersection + crosswalk
    const int E = 16;           //east
    const int W = 32;           //west
    const int N = 64;           //north
    const int S = 128;          //south
    const int SIDEWALK = 256;   //sidewalk
    const int CROSSWALK = 512;  //crosswalk

    DriverTree myTree;

    public float AvoidanceDistance = 6f;

    public bool isLawBreaker = false;

    // Do not for use for a truly free roaming AI
    // the bool constantly turns off and on for pathfinding purposes
    bool allowFreeRoam = false;
    public bool randomizeSpeeds = true;
    //public bool debugMessages = false;

    private NavMeshAgent agent;

    //unstuck timer
    float timer = 0;

    public float dropOffDistance = 0;

    public Vector3 currentDestination;
    public _PedestrianAI myPassenger;
    public bool hasPassenger = false;

    UberController uberController;

    Rigidbody rb;
    Collider col;

    Transform spherecast;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        col.isTrigger = true;
        rb.isKinematic = true;

        dropOffDistance = Random.Range(5, 10);
        uberController = GameObject.FindGameObjectWithTag("Uber Controller").GetComponent<UberController>();

        agent = GetComponent<NavMeshAgent>();

        //agent.destination = currentDestination;
        agent.stoppingDistance = 6;
        agent.avoidancePriority = Random.Range(1, 100);

        if (randomizeSpeeds)
        {
            agent.speed = Random.Range(8, 15);
            agent.acceleration = Random.Range(8, 15);
            AvoidanceDistance = Random.Range(6, 12);
        }

        myTree = gameObject.AddComponent<DriverTree>();

        foreach (Transform child in transform)
        {
            if (child.name == "SphereCast")
            {
                spherecast = child;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            // Traffic Light
            if (other.gameObject.tag == "Traffic")
            {
                if (other.gameObject.GetComponent<TrafficLight>().isRed == true)
                {
                    agent.isStopped = true;
                    //rb.isKinematic = false;
                    agent.ResetPath();
                }
            }

            if (other.gameObject.tag == "Walker")
            {
                // Pickup Stuff

                agent.isStopped = true;
                //isLawBreaker = true;
            }

            if (other.gameObject.tag == "Rider")
            {
                // Pickup Rider
                //myPassenger = other.GetComponent<_PedestrianAI>();
                //other.gameObject.transform.SetParent(gameObject.transform);
            }

            if (other.tag == "AI")
            {
                agent.isStopped = true;                
            }
        }
    }

    void OnTriggerStay(Collider collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.tag == "Traffic")
            {
                if (collision.gameObject.GetComponent<TrafficLight>().isRed == false)
                {
                    agent.isStopped = false;
                    agent.SetDestination(currentDestination);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other != null)
        {
            if (other.tag == "Traffic")
            {
                agent.isStopped = false;
                agent.SetDestination(currentDestination);
            }
        }
    }

    public bool IsAtDestination(float threshold = 0)
    {
        var dist = Vector3.Distance(agent.transform.position, currentDestination);

        if (threshold == 0)
        {            
            if (dist < dropOffDistance)
                return true;
        }
        else if (threshold != 0)
        {
            if (dist < threshold)
            {
                return true;
            }
        }

        return false;
    }

    void DoVehicleCollision()
    {

        //rb.useGravity = false;
        //rb.isKinematic = false;
        //rb.AddForce(-agent.velocity / 4, ForceMode.Impulse);
    }

    void AssignRoadLayers(int mask)
    {
        Vector3 coord = (currentDestination - transform.position).normalized;
        float x1 = Mathf.Abs(currentDestination.x);
        float x2 = Mathf.Abs(transform.position.x);
        float dist = Mathf.Abs(x1 - x2);
        //allowFreeRoam = false;        

        switch (mask)
        {
            // on intersection
            case I:
            case INTERSECTION:
            case CROSSWALK:
                //NW
                if (coord.z > 0 && coord.x < 0)
                {
                    if (dist < 30)
                    {
                        agent.areaMask = I + N + E;
                    }
                    else
                        agent.areaMask = I + N + W;
                }

                //NE
                if (coord.z > 0 && coord.x > 0)
                {
                    if (dist < 30)
                    {
                        agent.areaMask = I + N + W;
                    }
                    else
                        agent.areaMask = I + N + E;
                }

                //SW
                if (coord.z < 0 && coord.x < 0)
                {
                    if (dist < 30)
                    {
                        agent.areaMask = I + S + E;
                    }
                    else
                        agent.areaMask = I + S + W;
                }

                //SE
                if (coord.z < 0 && coord.x > 0)
                {
                    if (dist < 30)
                    {
                        agent.areaMask = I + S + W;
                    }
                    else
                        agent.areaMask = I + S + E;
                }
                break;

            // on east facing road
            case E:
                //NW
                if (IsAtDestination())
                {
                    if (coord.z > 0 && coord.x < 0)
                    {
                        agent.areaMask = I + N + E;
                    }

                    //NE
                    if (coord.z > 0 && coord.x > 0)
                    {
                        agent.areaMask = I + N + E;
                    }

                    //SW
                    if (coord.z < 0 && coord.x < 0)
                    {
                        agent.areaMask = I + S + E;
                    }

                    //SE
                    if (coord.z < 0 && coord.x > 0)
                    {
                        agent.areaMask = I + S + E;
                    }
                }
                break;

            //on west facing road
            case W:

                if (IsAtDestination())
                {

                    //NW
                    if (coord.z > 0 && coord.x < 0)
                    {
                        agent.areaMask = I + N + W;
                    }

                    //NE
                    if (coord.z > 0 && coord.x > 0)
                    {
                        agent.areaMask = I + N + W;
                    }

                    //SW
                    if (coord.z < 0 && coord.x < 0)
                    {
                        agent.areaMask = I + S + W;
                    }

                    //SE
                    if (coord.z < 0 && coord.x > 0)
                    {
                        agent.areaMask = I + S + W;
                    }
                }
                break;

            // on north facing road
            case N:

                if (IsAtDestination())
                {
                    //NW
                    if (coord.z > 0 && coord.x < 0)
                    {
                        agent.areaMask = I + N + W;
                    }

                    //NE
                    if (coord.z > 0 && coord.x > 0)
                    {
                        agent.areaMask = I + N + E;
                    }

                    //SW
                    if (coord.z < 0 && coord.x < 0)
                    {
                        agent.areaMask = I + N + W;
                    }

                    //SE
                    if (coord.z < 0 && coord.x > 0)
                    {
                        agent.areaMask = I + N + E;
                    }
                }
                break;

            //on south facing road
            case S:

                if (IsAtDestination())
                {
                    //NW
                    if (coord.z > 0 && coord.x < 0)
                    {
                        agent.areaMask = I + S + W;
                    }

                    //NE
                    if (coord.z > 0 && coord.x > 0)
                    {
                        agent.areaMask = I + S + E;
                    }

                    //SW
                    if (coord.z < 0 && coord.x < 0)
                    {
                        agent.areaMask = I + S + W;
                    }

                    //SE
                    if (coord.z < 0 && coord.x > 0)
                    {
                        agent.areaMask = I + S + E;
                    }
                }
                break;

            case 0:
            default:
                break;
        }
    }

    void UpdateRoadLayers()
    {

        NavMeshHit info;

        if (NavMesh.SamplePosition(agent.transform.position, out info, 8f, NavMesh.AllAreas))
        {
            AssignRoadLayers(info.mask);
        }

        if (allowFreeRoam)
        {
            agent.areaMask = I + N + S + E + W;
        }

        //////////////////////////////////////////////////////////////////
        // This is to prevent agent from stopping in middle of road 
        // because it cant reach the destination on other side        
        if (agent.isPathStale || agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            DoUnstuck();
        }

        else if (!agent.isPathStale && agent.velocity == Vector3.zero)
        {
            DoUnstuck();
        }

        else if (!IsAtDestination() && agent.velocity == Vector3.zero)
        {
            if (agent.isStopped == false)
            {
                timer += Time.deltaTime;
                if (timer > 3)
                {
                    DoUnstuck();
                    timer = 0;
                }
            }
        }
    }

    void DoUnstuck()
    {
        var dist = Vector3.Distance(transform.position, currentDestination);
        if (dist <= 14 && myPassenger != null)
        {
            Debug.Log("close. unstucking");
            //allowFreeRoam = true;
            agent.SetDestination(currentDestination);
        }

        if (agent.isStopped)
        {
            agent.isStopped = false;
        }
    }

    void AvoidObjects()
    {
        RaycastHit hit;

        Vector3 pos;
        if (spherecast != null)
            pos = spherecast.position;
        else
            pos = transform.position;

        bool res = Physics.SphereCast(pos, 2f, transform.forward, out hit, AvoidanceDistance);

        if (res)
        {
            if (hit.transform.gameObject.tag == "Walker" ||
                hit.transform.gameObject.tag == "AI")
            {
                agent.isStopped = true;
            }

            else
            {

            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        UpdateRoadLayers();
        AvoidObjects();

        if (uberController != null)
            myTree.RunTree(this);

        //////////////////////////////////////////////////////////////////        
        // Destination is assigned from Uber Controller
        //if (IsAtDestination() && agent.velocity == Vector3.zero)
        //{
        //    //prevPosition = currentDestination;
        //    //agent.SetDestination(currentDestination);
        //    allowFreeRoam = false;
        //}

    }
}