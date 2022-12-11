using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathKnightEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }

    public void Attack()
    {
        GetComponentInParent<DeathKnightSpecificEnemy>().LaunchBomb();
    }
}
