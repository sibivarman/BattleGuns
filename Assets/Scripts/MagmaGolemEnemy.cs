using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagmaGolemEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }
}
