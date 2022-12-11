using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonRoundEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }

    public void Attack()
    {
        GetComponentInParent<SkeletonRoundSpecificEnemy>().FireBullet();
    }
}
