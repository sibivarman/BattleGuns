using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonEnemy : SpecificEnemy
{
    public void DestroyEnemy()
    {
        GetComponentInParent<Enemy>().DestroyEnemy();
    }
}
