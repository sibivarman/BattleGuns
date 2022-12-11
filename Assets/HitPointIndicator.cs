using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPointIndicator : MonoBehaviour
{

    [SerializeField]
    private Animator animator;

    private GameObject bullet;

    private bool bulletHitFlag = true;

    private void Update()
    {
        if(bullet == null && bulletHitFlag)
        {
            bulletHitFlag = false;
            Invoke("CloseHitIndicator", 1);
        }
    }

    private void CloseHitIndicator()
    {
        animator.SetTrigger("Close");
    }

    public void SetBullet(GameObject _bulet)
    {
        bullet = _bulet;
    }

    private void DestroyIndicator()
    {
        Destroy(gameObject);
    }
}
