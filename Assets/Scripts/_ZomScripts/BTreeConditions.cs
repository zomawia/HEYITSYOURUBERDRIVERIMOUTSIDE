// BTreeConditions - specific nodes for behaviours
// by Zomawia Sailo

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BTreeConditions : BTreeNodes
{
    ///////////////////////////////////////////////////////// 
    // Passenger Nodes  

    public class IsTargetFar : IBTNode<_PedestrianAI>
    {
        public BTStatus execute(_PedestrianAI agent)
        {
            float dist = Vector3.Distance(agent.currentDestination, agent.transform.position);
            // do some stuff
            if (dist > agent.maxWalkRange)
            {
                //Debug.Log("PED: Target is far");                
                return BTStatus.success;
            }
            else
            {
                //Debug.Log("PED: Target is close enough");
                if (agent.GetComponent<NavMeshAgent>().enabled == false)
                {
                    agent.GetComponent<NavMeshAgent>().enabled = true;
                }
                agent.GetComponent<NavMeshAgent>().SetDestination(agent.currentDestination);
                return BTStatus.failure;
            }            
        }
    }

    public class IsInCar : IBTNode<_PedestrianAI>
    {
        public BTStatus execute(_PedestrianAI agent)
        {
            if (agent.isInCar == true)
            {
                //Debug.Log("PED: I'm in a car");
                return BTStatus.success;
            }
            else
            {
                //Debug.Log("PED: I'm not in a car");
                return BTStatus.failure;
            }
        }
    }

    public class RequestRide : IBTNode<_PedestrianAI>
    {
        public BTStatus execute(_PedestrianAI agent)
        {            
            if (agent.myRide == null)
            {
                //Debug.Log("PED: No driver set. Requesting a ride");
                agent.RequestRide();
            }

            if (agent.myRide != null)
            {
                //Debug.Log("PED: Driver is set. Success");
                return BTStatus.success;
            }

            //Debug.Log("PED: No drivers available");
            return BTStatus.failure;
        }
    }

    public class WaitForRide : IBTNode<_PedestrianAI>
    {
        public BTStatus execute(_PedestrianAI agent)
        {
            //Debug.Log("PED: Waiting for a ride. Idling.");
            agent.WaitForRide();
            return BTStatus.success;
        }
    }

    public class IsRideClose : IBTNode<_PedestrianAI>
    {
        public BTStatus execute(_PedestrianAI agent)
        {
            // dist stuff
            float dist = Vector3.Distance(agent.transform.position, agent.myRide.transform.position);
            if (dist < 1)
            {
                //Debug.Log("PED: My ride is close to me.");
                return BTStatus.success;
            }
            else if (dist < 35 && dist >= 1)
            {                
                //Debug.Log("PED: My ride is close enough. Walking remaining distance");
                if (agent.GetComponent<NavMeshAgent>().isStopped == true)
                {
                    //Debug.Log("PED: My navmesh was stopped. Starting it up.");
                    agent.GetComponent<NavMeshAgent>().isStopped = false;
                }
                agent.GetComponent<NavMeshAgent>().areaMask = -1;
                agent.GetComponent<NavMeshAgent>().SetDestination(agent.myRide.transform.position);
                return BTStatus.failure;
            }
            else
            {
                //Debug.Log("PED: My ride is far from me.");
                return BTStatus.failure;
            }
        }
    }

    public class GetInTheCar : IBTNode<_PedestrianAI>
    {
        public BTStatus execute(_PedestrianAI agent)
        {
            //Debug.Log("PED: Getting in the car.");
            agent.isInCar = true;
            agent.GetComponent<NavMeshAgent>().enabled = false;
            return BTStatus.success;

            //driver will parent the rider
        }
    }

    public class OutOfCar : IBTNode<_PedestrianAI>
    {
        public BTStatus execute(_PedestrianAI agent)
        {
            if (agent.isInCar == false)
            {
                //Debug.Log("PED: I'm out of the car");          
                
                return BTStatus.success;
            }
            return BTStatus.failure;
        }
    }

    public class IsDestinationClose : IBTNode<_PedestrianAI>
    {
        public BTStatus execute(_PedestrianAI agent)
        {
            float dist = Vector3.Distance(agent.transform.position, agent.currentDestination);
            if (dist <= agent.maxWalkRange)
            {
                //Debug.Log("PED: Destination is close. Walking there.");
                
                agent.gameObject.tag = "Walker";
                return BTStatus.success;
            }
            //Debug.Log("PED: Destination too far. ERROR");
            return BTStatus.failure;
        }
    }

    public class DoWalk : IBTNode<_PedestrianAI>
    {
        public BTStatus execute(_PedestrianAI agent)
        {
            if (agent.GetComponent<NavMeshAgent>().isStopped == true)
                agent.GetComponent<NavMeshAgent>().isStopped = false;

            if (agent.GetComponent<NavMeshAgent>().enabled == false)
                agent.GetComponent<NavMeshAgent>().enabled = true;

            //Debug.Log("PED: I am walking to my destination");
            agent.GetComponent<NavMeshAgent>().areaMask = 768;
            agent.GetComponent<NavMeshAgent>().SetDestination(agent.currentDestination);
            return BTStatus.success;
        }
    }

    public class IsAtDestination : IBTNode<_PedestrianAI>
    {
        public BTStatus execute(_PedestrianAI agent)
        {
            if (agent.IsAtDestination())
            {
                agent.myRide = null;                
                return BTStatus.success;
            }
            return BTStatus.failure;
        }
    }

    public class AssignNewDestination : IBTNode<_PedestrianAI>
    {
        public BTStatus execute(_PedestrianAI agent)
        {
            agent.SetNewDestination();
            return BTStatus.success;            
        }
    }


    /////////////////////////////////////////////////////////
    // DriverNodes

    public class HasPassenger : IBTNode<UberDriverAI>
    {
        public BTStatus execute(UberDriverAI agent)
        {
            if (agent.hasPassenger == true)
            {
                agent.myPassenger.GetComponent<NavMeshAgent>().enabled = false;
                //Debug.Log("DRIVER: I already have a passenger in the car");
                return BTStatus.success;
            }

            else
            {
                //Debug.Log("DRIVER: I have no passenger");
                return BTStatus.failure;
            }
        }
    }

    public class HasRequest : IBTNode<UberDriverAI>
    {
        public BTStatus execute(UberDriverAI agent)
        {
            if (agent.myPassenger != null)
            {
                //Debug.Log("DRIVER: I have a request");
                return BTStatus.success;
            }
            else
            {
                //Debug.Log("DRIVER: I have no requests");
                return BTStatus.failure;
            }
            
        }
    }
    public class DriveToRequest : IBTNode<UberDriverAI>
    {
        public BTStatus execute(UberDriverAI agent)
        {
            //Debug.Log("DRIVER: Driving to my rider");
            agent.currentDestination = agent.myPassenger.transform.position;
            agent.GetComponent<NavMeshAgent>().SetDestination(agent.currentDestination);
            return BTStatus.success;
        }
    }

    public class IsCloseToRequest : IBTNode<UberDriverAI>
    {
        public BTStatus execute(UberDriverAI agent)
        {
            float dist = Vector3.Distance(agent.transform.position, agent.myPassenger.transform.position);
            if (dist < 8)
            {
                //Debug.Log("DRIVER: I am close to pickup");
                return BTStatus.success;
            }
            //Debug.Log("DRIVER: Too far from pickup");
            return BTStatus.failure;
        }
    }

    public class PickupRequest : IBTNode<UberDriverAI>
    {
        public BTStatus execute(UberDriverAI agent)
        {
            if (agent.hasPassenger == false)
            {
                //Debug.Log("DRIVER: Picking up rider");
                agent.hasPassenger = true;
                agent.myPassenger.gameObject.transform.parent = agent.transform;
                //agent.myPassenger.gameObject.transform.localPosition = new Vector3(0, 14, 0);
                return BTStatus.success;
            }

            else
            {
                //Debug.Log("DRIVER: Error picking up rider");
                return BTStatus.failure;
            }
        }
    }

    //Step 2 Sequence - Drive to Destination
    public class DriveToDestination : IBTNode<UberDriverAI>
    {
        public BTStatus execute(UberDriverAI agent)
        {
            //Debug.Log("DRIVER: Driving to the destination");            

            if (agent.myPassenger.currentDestination != agent.currentDestination)
            {
                //Debug.Log("DRIVER: Setting the destination");
                agent.currentDestination = agent.myPassenger.currentDestination;
                agent.GetComponent<NavMeshAgent>().SetDestination(agent.currentDestination);                
            }
            agent.myPassenger.transform.position = agent.transform.position;
            
            
            return BTStatus.success;
        }
    }

    public class IsDriverAtDestination : IBTNode<UberDriverAI>
    {
        public BTStatus execute(UberDriverAI agent)
        {
            if (agent.IsAtDestination())
            {
                //Debug.Log("DRIVER: Arrived at destination");
                agent.currentDestination = agent.transform.position;
                agent.GetComponent<NavMeshAgent>().destination = agent.transform.position;
                return BTStatus.success;                
            }

            //Debug.Log("DRIVER: Not yet at destination");
            return BTStatus.failure;
        }
    }

    public class RemovePassenger : IBTNode<UberDriverAI>
    {
        public BTStatus execute(UberDriverAI agent)
        {
            //Debug.Log("DRIVER: Ride complete. Removing passenger");

            agent.myPassenger.gameObject.transform.parent = null;
            agent.myPassenger.isInCar = false;
            agent.myPassenger.myRide = null;
            agent.myPassenger = null;
            agent.hasPassenger = false;
            return BTStatus.success;
        }
    }
}
