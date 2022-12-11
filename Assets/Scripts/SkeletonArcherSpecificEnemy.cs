using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SkeletonArcherSpecificEnemy : SpecificEnemy
{
    [SerializeField]
    private float radius = 20;

    [SerializeField]
    private float waitTime = 1;

    [SerializeField]
    private GameObject lavaBulletGOPrefab;

    [SerializeField]
    private GameObject hitPointIndicatorPrefab;

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

    private int IDLE_ANIMATION = Animator.StringToHash("Idle");

    private int Attack_ANIMATION_HASH = Animator.StringToHash("Attack");

    private int MOVE_ANIMATION = Animator.StringToHash("Move");


    void Start()
    {
        ai = GetComponent<IAstarAI>();
        startPos = ai.position;
    }

    void Update()
    {
        //if the player completed the path check whther has to attack or wait for random amount of time
        //after waiting for random amount of time check whether has to attack or move to random point

        if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath) && !enemyIdle && !isAttacking)
        {
            enemyIdle = true;
            animator.ResetTrigger(MOVE_ANIMATION);
            animator.SetTrigger(IDLE_ANIMATION);
            Invoke("CalculateNextPath", waitTime);
        }
        else if (!enemyIdle && !isAttacking)
        {
            animator.SetTrigger(MOVE_ANIMATION);
        }
        if (turnTowardPlayer)
        {
            Vector3 pos = -transform.position + target.position;
            pos.y = 0;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(pos), Time.deltaTime * rotationSpeed);
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
        Invoke("CompleteAttack", 1);
    }

    public void FireBullet()
    {
        GameObject bullet = Instantiate(lavaBulletGOPrefab, firingPoint.position, firingPoint.rotation);
        //bullet.transform.position = new Vector3(firingPoint.position.x, 0.5f, firingPoint.z);
        bullet.GetComponent<Bullet>().Fire(target.transform);
        GameObject hitPointIndicator = Instantiate(hitPointIndicatorPrefab, new Vector3(target.transform.position.x, hitPointIndicatorPrefab.transform.position.y, target.transform.position.z), hitPointIndicatorPrefab.transform.rotation);
        hitPointIndicator.GetComponent<HitPointIndicator>().SetBullet(bullet);
        turnTowardPlayer = false;
    }

    private void CompleteAttack()
    {
        isAttacking = false;
        animator.SetTrigger(MOVE_ANIMATION);
        ai.destination = PickRandomPoint();
        ai.SearchPath();
    }

    private void CalculateNextPath()
    {
        enemyIdle = false;
        if (canAttack)
        {
            turnTowardPlayer = true;
            Attack();
        }
    }

    Vector3 PickRandomPoint()
    {
        var point = Random.insideUnitSphere * radius;
        point.y = 0;
        point += startPos;
        startPos.y = 0.5f;
        return point;
    }
}
