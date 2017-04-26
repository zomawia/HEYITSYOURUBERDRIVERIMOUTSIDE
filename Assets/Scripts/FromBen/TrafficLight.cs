using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    public float enableTimer = 10;
    public float timerDelay = 20;
    public float disablePause = 10;

    public bool isRed = true;

	// Use this for initialization
	void Start ()
    {
		//yes
	}
	
	// Update is called once per frame
	void Update ()
    {
        //GetComponent<Collider>().enabled = true;
        enableTimer -= Time.deltaTime;

        if (enableTimer <= 0)
        {
            //GetComponent<Collider>().enabled = false;
            isRed = false;
            disablePause -= Time.deltaTime;
            if(disablePause <= 0)
            {
                isRed = true;
                enableTimer += timerDelay;
                disablePause += 10;
            }
        }
	}
}
