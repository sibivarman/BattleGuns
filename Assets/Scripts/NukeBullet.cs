using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NukeBullet : IBullet
{

    [SerializeField]
    private float speed;

    [SerializeField]
    private Rigidbody rb;

    public override void Fire(Transform target)
    {
        //base.Fire(target);
        Vector3 dir = target.position - transform.position;
        rb.velocity = dir.normalized * speed;
    }
}
