using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeamonWolfEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }
}
