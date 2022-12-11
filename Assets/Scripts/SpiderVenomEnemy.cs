using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderVenomEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }

    public void Attack()
    {
        GetComponentInParent<SpiderVenomSpecificEnemy>().FireBullet();
    }
}
