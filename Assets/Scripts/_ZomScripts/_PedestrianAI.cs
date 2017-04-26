// PedestrianAI 
// by Zomawia Sailo
// Agent will go to a destination using a limited amount of "walkable" mask
// Attempts to evade incoming vehicles
// Requests a ride if its destination is too far

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class _PedestrianAI : MonoBehaviour {

    PassengerTree myTree;

    const int I = 8;    // intersection (peds probably shouldnt use it)
    const int S = 256;  // sidewalk
    const int C = 512;  // crosswalk
   
    public Vector3 currentDestination;    

    public GameObject myRide;
    public bool isInCar = false;

    public bool RandomizeSpeeds = true;
    private NavMeshAgent agent;    

    public float maxWalkRange = 0;

    Rigidbody rb;
    GameObject wpc;    
    UberController uberController;
    Collider col;

    private void Awake()
    {

        if (GameObject.FindGameObjectWithTag("Uber Controller"))
        {
            uberController = GameObject.FindGameObjectWithTag("Uber Controller").GetComponent<UberController>();
            myTree = gameObject.AddComponent<PassengerTree>();
        }
    }

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        col.isTrigger = true;
        rb.isKinematic = true;

        if (GameObject.FindGameObjectWithTag("Waypoint Controller"))
            wpc = GameObject.FindGameObjectWithTag("Waypoint Controller");

        // use ubercontroller and behavior tree only if it exists in the game


        maxWalkRange = Random.Range(50, 200);

        if (GetComponent<NavMeshAgent>())
        {
            agent = GetComponent<NavMeshAgent>();
            agent.areaMask = C + S;
            agent.stoppingDistance = 2;

            // Helps to stop the AI from bumping into each other
            agent.avoidancePriority = Random.Range(1, 100);
        }
        currentDestination = wpc.GetComponent<WaypointController>().GetRandomDestination().position;        

        if (RandomizeSpeeds)
        {
            agent.speed = Random.Range(2, 4);
            agent.acceleration = Random.Range(2, 4);
        }        
    }

    public bool IsAtDestination()
    {
        var dist = Vector3.Distance(agent.transform.position, currentDestination);
        if (dist < 12)
            return true;

        return false;
    }

    public void SetNewDestination()
    {
        currentDestination = wpc.GetComponent<WaypointController>().GetRandomDestination().position;
        //agent.SetDestination(currentDestination);
    }

    private void OnTriggerEnter(Collider other)
    {       
        if (other.tag == "AI")
        {
            Vector3 force = other.GetComponent<NavMeshAgent>().velocity;

            rb.isKinematic = false;
            gameObject.GetComponent<NavMeshAgent>().enabled = false;

            rb.AddExplosionForce(8f, transform.position, 3f, 1f, ForceMode.Impulse);
            rb.AddForce(force, ForceMode.Impulse);
            rb.AddTorque(new Vector3(5, 5, 5), ForceMode.Impulse);
        }        
    }

    public void RequestRide()
    {
        if (uberController != null)
        {
            myRide = uberController.GetAvailableDriver();

            float dist = Vector3.Distance(transform.position, currentDestination);
            if (myRide == null && (dist <= maxWalkRange * 4))
            {
                if (agent.enabled == true)
                    agent.SetDestination(currentDestination);
            }
        }
    }

    public void WaitForRide()
    {
        if (myRide != null)
        {
            myRide.GetComponent<UberDriverAI>().myPassenger = this;
            gameObject.tag = "Rider";
            uberController.riders.Add(gameObject);
            agent.isStopped = true;
        }
    }

    void EvadeFromVehicles()
    {
        RaycastHit hitForward, hitRight, hitLeft;
        bool resA = Physics.SphereCast(transform.position, 2f, transform.right, out hitRight, 5f);
        bool resB = Physics.SphereCast(transform.position, 2f, -transform.right, out hitLeft, 5f);
        bool resC = Physics.SphereCast(transform.position, 2f, transform.forward, out hitForward, 6f);

        //Vector3 dir = (target.position + targetRb.velocity - transform.position);
        //Vector3 force = -dir.normalized * speed - rb.velocity;

        if (resA || resB || resC)
        {
            if (resA && hitRight.transform.tag == "AI")
            {
                NavMeshAgent car = hitRight.transform.GetComponent<NavMeshAgent>();
                Vector3 dir = (car.transform.position + car.velocity - transform.position);
                Vector3 force = -dir.normalized * agent.speed*1.5f - agent.velocity;

                agent.velocity += force;
            }
            else if (resB && hitLeft.transform.tag == "AI")
            {
                NavMeshAgent car = hitLeft.transform.GetComponent<NavMeshAgent>();
                Vector3 dir = (car.transform.position + car.velocity - transform.position);
                Vector3 force = -dir.normalized * agent.speed*1.5f - agent.velocity;

                agent.velocity += force;
            }
            else if (resC && hitForward.transform.tag == "AI")
            {
                NavMeshAgent car = hitForward.transform.GetComponent<NavMeshAgent>();
                Vector3 dir = (car.transform.position + car.velocity - transform.position);
                Vector3 force = -dir.normalized * agent.speed*1.5f - agent.velocity;

                agent.velocity += force;
            }
        }
    }

    // Update is called once per frame
    void Update () {

        if (uberController != null)
            myTree.RunTree(this);

        if (isInCar)
        {
            agent.enabled = false;
        }
        else
        {
            if (agent)
                agent.enabled = true;

            EvadeFromVehicles();

            if (rb.isKinematic == false && transform.position.y < -50)
                Destroy(gameObject);

            // get new Destination
            if (IsAtDestination() && agent.velocity == Vector3.zero)
            {
                //prevDestination = currentDestination;
                currentDestination = wpc.GetComponent<WaypointController>().GetRandomDestination().position;
                agent.SetDestination(currentDestination);
            }
        }
    }
}
