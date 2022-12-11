using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderlingVenomEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }
}
