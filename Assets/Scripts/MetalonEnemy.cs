using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalonEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }
}
