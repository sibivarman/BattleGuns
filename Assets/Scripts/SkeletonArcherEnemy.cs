using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonArcherEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }

    public void Attack()
    {
        GetComponentInParent<SkeletonArcherSpecificEnemy>().FireBullet();
    }
}
