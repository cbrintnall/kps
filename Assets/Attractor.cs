using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    public LayerMask Targets;
    public float Radius = 10.0f;
    public float Force = 7.5f;

    Transform target;
    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (target)
        {
            rb.AddForce(
                (target.transform.position - transform.position).normalized * Force,
                ForceMode.VelocityChange
            );
        }
        else
        {
            var hits = Physics.OverlapSphere(transform.position, Radius, Targets);

            if (hits.Length > 0)
            {
                Debug.Log("Found target");
                target = hits.First().gameObject.transform;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, Radius);
    }
}
