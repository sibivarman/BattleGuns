using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneEyedWizzardEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }

    public void LaunchLightiningBomb()
    {
        GetComponentInParent<OneEyedWizzardSpecificEnemy>().LaunchBomb();
    }
}
