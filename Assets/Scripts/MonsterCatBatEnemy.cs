using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCatBatEnemy : MonoBehaviour
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }
}
