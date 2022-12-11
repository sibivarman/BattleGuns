using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FlowerMonsterSpecificEnemy : SpecificEnemy
{
    [SerializeField]
    private float radius = 20;

    [SerializeField]
    private float waitTime = 1;

    [SerializeField]
    private GameObject lavaBulletGOPrefab;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform firingPoint;

    [SerializeField]
    private float rotationSpeed;

    private IAstarAI ai;

    private Vector3 startPos;

    private Transform target;

    private bool canAttack;

    private bool enemyIdle;

    private bool isAttacking;

    private bool turnTowardPlayer;

    private Vector3 pos;

    private int IDLE_ANIMATION = Animator.StringToHash("Idle");

    private int Attack_ANIMATION_HASH = Animator.StringToHash("Attack");

    private int MOVE_ANIMATION = Animator.StringToHash("Move");


    void Start()
    {
        ai = GetComponent<IAstarAI>();
        startPos = ai.position;
        target = GetComponent<Enemy>().GetTarget();
    }

    void Update()
    {
        pos = -transform.position + target.position;
        pos.y = 0;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(pos), Time.deltaTime * rotationSpeed);
        if (canAttack)
        {
            Attack();
        }
    }

    public override void Attack(Transform _target)
    {
        target = _target;
        canAttack = true;
    }

    private void Attack()
    {
        canAttack = false;
        isAttacking = true;
        animator.SetTrigger(Attack_ANIMATION_HASH);
        animator.SetTrigger(IDLE_ANIMATION);
    }

    public void FireBullet()
    {
        GameObject bullet = Instantiate(lavaBulletGOPrefab, firingPoint.position, firingPoint.rotation);
        bullet.GetComponent<Bullet>().Fire(target.transform);
        turnTowardPlayer = false;
    }
}
