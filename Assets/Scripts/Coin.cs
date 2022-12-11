using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    [SerializeField]
    private float explosionForce;

    [SerializeField]
    private float radius;

    [SerializeField]
    private float upwardModifier;

    [SerializeField]
    private float waitAfterExplosionTime;

    [SerializeField]
    private float coinSpeed;

    [SerializeField]
    private AudioClip coinSound;

    private Transform target;

    private bool followEnabled;

    private void Update()
    {
        if (followEnabled)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, coinSpeed * Time.deltaTime);
        }
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    public void Explode(Vector3 pos)
    {
        GetComponent<Rigidbody>().AddExplosionForce(explosionForce, pos, radius, upwardModifier,ForceMode.Impulse);
        Invoke("FollowTarget", waitAfterExplosionTime);
    }

    private void FollowTarget()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
        followEnabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            followEnabled = false;
            GetComponent<BoxCollider>().isTrigger = false;
            GetComponent<Rigidbody>().isKinematic = false;
            if (AudioManager.isSoundEnabled)
            {
                AudioSource.PlayClipAtPoint(coinSound, transform.position);
            }
            GetComponentInParent<CoinPoolManager>().CollectCoin(gameObject);
        }
    }

}
