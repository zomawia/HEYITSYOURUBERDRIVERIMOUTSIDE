// Waypoint by James Lyle

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
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
    void Awake()
    {
        InitWaypointList();
    }

    void Update()
    {

    }
}