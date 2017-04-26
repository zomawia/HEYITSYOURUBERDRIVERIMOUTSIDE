using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekTest : MonoBehaviour
{

    Rigidbody rb;
    public Transform target;
    public Transform traffic;
    public float speed;
    public TrafficLight tl;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        var dir = (target.position - transform.position).normalized;

        var desiredVelocity = dir * speed;

        var force = desiredVelocity - rb.velocity;

        rb.AddForce(force);

        //if (tl.isRed == true)
        //{
        //    ItsTimeToStop();
        //}
        if (tl.isRed == false)
        {
            ItsTimeToGo();
        }
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
            ItsTimeToStop();
        }
    }

    void ItsTimeToStop()
    {
        speed = 0;
    }

    void ItsTimeToGo()
    {
        speed = 30;
    }
}
