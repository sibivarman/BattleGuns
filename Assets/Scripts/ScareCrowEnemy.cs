using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScareCrowEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }

    public void Attack()
    {
        GetComponentInParent<ScareCrowSpecificEnemy>().FireBullet();
    }
}
