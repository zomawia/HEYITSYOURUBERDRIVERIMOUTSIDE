// Basic AI Controller 
// by Zomawia Sailo
// For vehicles using a road system

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// dot(fwd, dis) < cos(fov/2)

public class _BaseAIController : MonoBehaviour {

    // area mask bit codes
    const int INTERSECTION = 8;
    const int I = 520;          //intersection + crosswalk
    const int E = 16;           //east
    const int W = 32;           //west
    const int N = 64;           //north
    const int S = 128;          //south
    const int SIDEWALK = 256;   //sidewalk
    const int CROSSWALK = 512;  //crosswalk

    public float AvoidanceDistance = 6f;

    public bool isLawBreaker = false;
    float lawBreakerResetTimer = 0;
    public bool RandomizeSpeeds = true;

    // Do not for use for a truly free roaming AI
    // the bool constantly turns off and on for pathfinding purposes
    bool allowFreeRoam = false;

    public bool debugMessages = false;    

    private NavMeshAgent agent;    

    //unstuck timer
    float timer = 0;

    Vector3 startPos;
    Vector3 currentDestination;

    Rigidbody rb;
    GameObject wpc;
    Collider col;

    Transform spherecast;

    GameObject ExplosionVFX;

    int hitCounter = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        col.isTrigger = true;
        rb.isKinematic = true;
        wpc = GameObject.FindGameObjectWithTag("Waypoint Controller");

        startPos = transform.position;

        if (Resources.Load("ExplosionEffect"))
            ExplosionVFX = (GameObject)Resources.Load("ExplosionEffect");

        agent = GetComponent<NavMeshAgent>();

        agent.destination = currentDestination = wpc.GetComponent<WaypointController>().GetRandomDestination().position;

        agent.stoppingDistance = 2;

        if (RandomizeSpeeds)
        {
            agent.speed = Random.Range(8, 15);
            agent.acceleration = Random.Range(8, 15);
            AvoidanceDistance = Random.Range(6, 12);
        }

        // Helps to stop the AI from bumping into each other
        agent.avoidancePriority = Random.Range(1, 100);

        foreach (Transform child in transform)
        {
            if (child.name == "SphereCast")
            {
                spherecast = child;
            }
        }
    }

    void Explode()
    {
        if (ExplosionVFX != null)
        {
            Instantiate(ExplosionVFX, transform.position, transform.rotation);
            //ExplosionVFX.transform.parent = transform;
            transform.position = startPos;
            hitCounter = 0;
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
                agent.isStopped = true;
                isLawBreaker = true;
            }

            if (other.tag == "AI")
            {
                agent.isStopped = true;
                DoVehicleCollision();
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

    public bool IsAtDestination()
    {
        var dist = Vector3.Distance(agent.transform.position, currentDestination);
        if (dist < 10)
            return true;

        return false;
    }

    void DoVehicleCollision()
    {
        hitCounter++;

        //rb.useGravity = false;
        //rb.isKinematic = false;
        //rb.AddForce(-agent.velocity / 4, ForceMode.Impulse);
        if (hitCounter > 2)
        {
            Explode();
        }
    } 

    // Logic to prevent vehicles turning on the wrong road
    void AssignRoadLayers(int mask)
    {
        Vector3 coord = (currentDestination - transform.position).normalized;
        //float x1 = Mathf.Abs(currentDestination.x);
        //float x2 = Mathf.Abs(transform.position.x);
        //float dist = Mathf.Abs(x1 - x2);
        //allowFreeRoam = false;

        switch (mask)
        {
            // on intersection
            case I:
            case INTERSECTION:
            //case CROSSWALK:
                //NW
                if (coord.z > 0 && coord.x < 0)
                {
                    //if (dist < 30)
                    //{
                    //    agent.areaMask = I + N + E;
                    //}
                    //else
                        agent.areaMask = I + N + W;
                }

                //NE
                if (coord.z > 0 && coord.x > 0)
                {
                    //if (dist < 30)
                    //{
                    //    agent.areaMask = I + N + W;
                    //}                    
                    //else
                        agent.areaMask = I + N + E;
                }

                //SW
                if (coord.z < 0 && coord.x < 0)
                {
                    //if (dist < 30)
                    //{
                    //    agent.areaMask = I + S + E;
                    //}
                    //else
                        agent.areaMask = I + S + W;                    
                }

                //SE
                if (coord.z < 0 && coord.x > 0)
                {
                    //if (dist < 30)
                    //{
                    //    agent.areaMask = I + S + W;
                    //}
                    //else
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
    }

    void DoUnstuck()
    {
        var dist = Vector3.Distance(transform.position, currentDestination);
        if (dist <= 14)
        {
            allowFreeRoam = true;
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
            if (hit.transform.gameObject.tag == "Walker" || hit.transform.gameObject.tag == "ZomDriver" ||
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

        //////////////////////////////////////////////////////////////////        
        // Destination is assigned from waypoint controller
        if (IsAtDestination() && agent.velocity == Vector3.zero)
        {
            //prevPosition = currentDestination;
            currentDestination = wpc.GetComponent<WaypointController>().GetRandomDestination().position;
            agent.SetDestination(currentDestination);            
            allowFreeRoam = false;
        }

        
        if (isLawBreaker)
        {
            gameObject.tag = "LawBreaker";

            lawBreakerResetTimer += Time.deltaTime;
            if (lawBreakerResetTimer > 10)
            {
                isLawBreaker = false;
                gameObject.tag = "AI";
            }

        }

        if (debugMessages)
        {
            print ("Path Status: " + agent.pathStatus);
            print("Is Path Stale: " + agent.isPathStale);
            print(IsAtDestination());
            print(currentDestination);
            print(agent.velocity);
            
        }        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}
