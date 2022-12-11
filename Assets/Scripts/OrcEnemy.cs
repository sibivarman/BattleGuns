using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcEnemy : SpecificEnemy
{

    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }

    public void Attack()
    {
        GetComponentInParent<OrcSpecificEnemy>().FireBullet();
    }

}
