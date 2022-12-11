using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcSlingerEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }
}
