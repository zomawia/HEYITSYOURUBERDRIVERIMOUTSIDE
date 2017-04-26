using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    float speed = 2f;

    float xForce;
    float yForce;
    float zForce;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        xForce = Input.GetAxis("Horizontal") * speed;
        yForce = Input.GetAxis("OtherDirection") * speed;
        zForce = Input.GetAxis("Vertical") * speed;
        
        transform.Translate(new Vector3(xForce, yForce, zForce));
    }
}
