using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingCobraSpecificEnemy : SpecificEnemy
{

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform firingPoint;

    [SerializeField]
    private GameObject acidBulletGOPrefab;

    private bool isVisibleInCamera;

    private Camera mainCamera;

    private Transform target;

    private int attackAnimationHash = Animator.StringToHash("Attack");

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public override void Attack(Transform _target)
    {
        //base.Attack();
        target = _target;
        animator.SetTrigger(attackAnimationHash);
        //if (IsVisibleInCamera())
        //{
        //}
    }

    public void LaunchBomb()
    {
        GameObject bullet = Instantiate(acidBulletGOPrefab, firingPoint.position, acidBulletGOPrefab.transform.rotation);
        bullet.GetComponent<Bullet>().Fire(target.transform);
    }

    private bool IsVisibleInCamera()
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
        return screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height;
    }
}
