using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PoliceAI : MonoBehaviour
{

    Rigidbody rb;
    // for regular waypoints
    public Transform target;

    // for pursuit target
    Vector3 currentDest;
    public Transform lawBreaker;
    
    public Transform traffic;
    public TrafficLight tl;
    //public LawBreakCheck lbc;
    //_BaseAIController baic;
    NavMeshAgent agent;
    GameObject wpc;

    float timer = 0;

    // Use this for initialization
    void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        wpc = GameObject.FindGameObjectWithTag("Waypoint Controller");
        agent.destination = currentDest = wpc.GetComponent<WaypointController>().GetRandomDestination().position;
        //rb = GetComponent<Rigidbody>();
        agent.speed = 5;
    }

    void DetectLawBreakers()
    {
        RaycastHit info;
        bool res = Physics.SphereCast(transform.position, 6f, transform.forward, out info, 40f);

        if (res != false)
        {
            if (info.transform.tag == "AI")
            {
                if(info.transform.GetComponent<_BaseAIController>())
                { 
                    if (info.transform.GetComponent <_BaseAIController>().isLawBreaker)
                    {
                        lawBreaker = info.transform;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        DetectLawBreakers();

        // pursuit mode
        if (lawBreaker != null)
        {
            agent.SetDestination(lawBreaker.position);

            agent.speed = 17f;
            agent.acceleration = 12;
            agent.angularSpeed = 150;

            Debug.Log("I FOUND YOU");
            //agent.SetDestination(lawBreaker.position);
        }
        else
        {
            agent.SetDestination(currentDest);
        }

        if(tl != null)
        {
            if (tl.isRed == false)
            {
                ItsTimeToGo();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Traffic")
        {
            ItsTimeToStop();
            Debug.Log("IT'S TIME TO STOP");
        }

        if (other.gameObject.tag == "LawBreaker")
        {
            DeleteLawbreaker();
            ItsTimeToStop();
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "LawBreaker")
        {
            ItsTimeToStop();
            DeleteLawbreaker();
            Debug.Log("BYE");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "LawBreaker")
        {
            agent.isStopped = false;
        }

        if (other.gameObject.tag == "Traffic")
        {
            ItsTimeToGo();
        }
    }

    void DeleteLawbreaker()
    {
        if (lawBreaker != null)
        {
            lawBreaker.GetComponent<NavMeshAgent>().isStopped = true;

            timer += Time.deltaTime;
            if (timer >= 5f)
            {
                DestroyObject(lawBreaker.gameObject);
                lawBreaker = null;
            }
        }
    }

    void ItsTimeToStop()
    {
        agent.ResetPath();
        Debug.Log("STOP!!");
    }

    void ItsTimeToGo()
    {
        agent.isStopped = false;
        Debug.Log("GO!!");
    }
}
