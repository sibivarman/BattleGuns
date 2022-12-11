using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonHandEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }
}
