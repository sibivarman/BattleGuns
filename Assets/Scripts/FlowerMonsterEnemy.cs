using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerMonsterEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }

    public void Attack()
    {
        GetComponentInParent<FlowerMonsterSpecificEnemy>().FireBullet();
    }
}
