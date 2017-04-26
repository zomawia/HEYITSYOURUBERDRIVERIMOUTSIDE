using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LawBreakCheck : MonoBehaviour
{
    public bool lawBreakCheck = false;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Traffic"))
        //{
        //    speed = 0;
        //    rb.velocity = Vector3.zero;
        //    Debug.Log("Rice");
        //}

        //Debug.Log("Rice");

        if (other.gameObject.tag == "Traffic")
        {
            lawBreakCheck = true;
        }
    }
}
