using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trafficlight : MonoBehaviour
{
    public float enableTimer = 10;
    public float timerDelay = 20;
    public float disablePause = 10;

    public bool isRed = true;

    void Start()
    {

    }
    void Update()
    {
        
        enableTimer -= Time.deltaTime;

        if (enableTimer <= 0)
        {
            isRed = false;
            disablePause -= Time.deltaTime;
            if (disablePause <= 0)
            {
                isRed = true;
                enableTimer += timerDelay;
                disablePause += 10;
            }
        }
    }
}
