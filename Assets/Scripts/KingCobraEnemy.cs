using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingCobraEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }

    public void LaunchAcidBomb()
    {
        GetComponentInParent<KingCobraSpecificEnemy>().LaunchBomb();
    }
}
