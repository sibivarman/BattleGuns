using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.GetComponent<Player>())
    //    {
    //        collision.gameObject.GetComponent<Player>().Hit(GetComponentInParent<Bullet>().GetDamage());
    //    }
    //}
}
