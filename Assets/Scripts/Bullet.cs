using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Bullet : MonoBehaviour
{
    private enum FireType { PARABOLIC, STRAIGHT, FORWARD , CUSTOM};
    private enum DamageType { SINGLE_TARGET, AREA_TARGET};
    private enum BulletType { SERIAL, PARALLEL};
    private enum Shooter { ENEMY, PLAYER};

    [SerializeField]
    private float fireRate = 1;

    [SerializeField]
    private float speed;

    [SerializeField]
    private int damage;

    [SerializeField]
    private FireType fireType;

    [ShowIf("fireType", FireType.PARABOLIC)]
    [SerializeField]
    private float angle;

    [SerializeField]
    private DamageType damageType;

    [ShowIf("damageType", DamageType.AREA_TARGET)]
    [SerializeField]
    private float areaRadius;

    [SerializeField]
    private BulletType bulletType;

    [SerializeField]
    private Shooter shooter;

    [SerializeField]
    private GameObject explosionGO;

    [SerializeField]
    private Rigidbody rb;

    [ShowIf("fireType", FireType.CUSTOM)]
    [SerializeField]
    private IBullet customBullet;

    private int enemyLayerMask;

    private bool playerFired;

    private void Start()
    {
        playerFired = shooter.Equals(Shooter.PLAYER);
        if (playerFired)
        {
            enemyLayerMask = 1 << 10;
        }
        else
        {
            enemyLayerMask = 1 << 8;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (!damageType.Equals(DamageType.SINGLE_TARGET))
        {
            Collider[] enemyColliders = Physics.OverlapSphere(transform.position, areaRadius, enemyLayerMask);
            foreach (Collider collider in enemyColliders)
            {
                if(playerFired && collider.GetComponent<Enemy>())
                    collider.GetComponent<Enemy>().Hit(damage);
                else if(collider.GetComponent<Player>())
                    collider.GetComponent<Player>().Hit(damage);
            }
        }
        else if (!playerFired && collision.gameObject.GetComponent<Player>())
        {
            collision.gameObject.GetComponent<Player>().Hit(damage);
        }
        Instantiate(explosionGO, transform.position, transform.rotation);
        Destroy(gameObject);

    }

    public void Fire(Transform target)
    {
        if (fireType.Equals(FireType.STRAIGHT))
        {
            Vector3 dir = target.position - transform.position;
            dir.y = 0;
            rb.velocity = dir.normalized * speed;
            //rb.velocity = transform.forward * speed;
        }
        else if(fireType.Equals(FireType.PARABOLIC))
        {
            Vector3 direction = target.position - transform.position;
            float height = direction.y;
            direction.y = 0;
            float distance = direction.magnitude;
            float angleInRad = angle * Mathf.Deg2Rad;
            direction.y = distance * Mathf.Tan(angleInRad);
            distance += height / Mathf.Tan(angleInRad);
            Vector3 velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * angleInRad)) * direction.normalized;
            //At close distance between player and enemy the tan produces NaN value which cannot be used as velocity, so at extremely close range direct hit is used,
            //since there is no difference when hit direclty at close range
            if(float.IsNaN(velocity.x) || float.IsNaN(velocity.y) || float.IsNaN(velocity.z))
            {
                rb.velocity = transform.forward * speed;
            }
            else
            {
                rb.velocity = velocity;
            }
        }
        else if (fireType.Equals(FireType.FORWARD))
        {
            rb.velocity = transform.forward * speed;
        }
        else
        {
            customBullet.Fire(target);
        }
    }

    public bool IsAreaAttack()
    {
        if (damageType.Equals(DamageType.AREA_TARGET))
            return true;
        return false;

    }

    public bool IsSingleBulletType()
    {
        if (bulletType.Equals(BulletType.SERIAL))
            return true;
        return false;
    }

    public float GetFireRate()
    {
        return fireRate;
    }

    public int GetDamage()
    {
        return damage;
    }
}

public class IBullet : MonoBehaviour
{
    public virtual void Fire(Transform target) { }
}
