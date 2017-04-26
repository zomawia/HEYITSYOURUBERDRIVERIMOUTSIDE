// UberController
// by Zomawia Sailo

// Organizes the drivers and passengers into a list
// to assign and remove availability of objects
// This script should be assigned to a top-level empty GameObject in the scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UberController : MonoBehaviour {

    public List<GameObject> drivers = new List<GameObject>();
    public List<GameObject> riders = new List<GameObject>();

    GameObject nextAvailable;

	// Use this for initialization
	void Awake () {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ZomDriver"))
        {
            drivers.Add(obj);
        }

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Rider"))
        {
            riders.Add(obj);
        }
	}

    public GameObject GetAvailableDriver()
    {
        foreach(GameObject obj in drivers)
        {
            if (obj.GetComponent<UberDriverAI>().myPassenger == null)
            {
                nextAvailable = obj;
                //obj.GetComponent<UberDriverAI>().hasPassenger = true;
                return nextAvailable;
            }            
        }
        return null;
    }

	// Update is called once per frame
	void Update () {
        
        // List cleanup
        riders = riders.Distinct().ToList();
        riders = riders.Where(item => item != null).ToList();
    }
}
