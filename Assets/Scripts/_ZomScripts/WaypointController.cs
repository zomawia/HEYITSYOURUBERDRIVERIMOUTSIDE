// WaypointController
// by Zomawia Sailo
// Holds a list of waypoints (destinations) on the level that
// objects can request from the Controller

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointController : MonoBehaviour {

    // Tag the controller as WPController
    // Make a blank GameObject and add this script to it
    // Add the waypoints as childs with "Waypoints" tag
    // The script will find the waypoints and add them to its list

    public List<Transform> waypointList = new List<Transform>();

    void InitWaypointList()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Waypoints"))
            {
                waypointList.Add(child);
            }
        }
    }

    public Transform GetRandomDestination()
    {
        return waypointList[Random.Range(0, waypointList.Count - 1)];
    }

	// Needs to be Awake so list is done before AI starts grabbing vectors
	void Awake () {
        InitWaypointList();                
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}