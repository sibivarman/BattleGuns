using UnityEngine;
using Pathfinding;

public class OrcSpecificEnemy : SpecificEnemy
{
    [SerializeField]
    private float radius = 20;

    [SerializeField]
    private float waitTime = 1;

    [SerializeField]
    private GameObject tornadoBulletGOPrefab;

    [SerializeField]
    private Animator animator;

    private IAstarAI ai;

    private Vector3 startPos;

    private Transform target;

    private bool canAttack;

    private bool enemyIdle;

    private bool isAttacking;

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
        else if(!enemyIdle && !isAttacking)
        {
            animator.SetTrigger(MOVE_ANIMATION);
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
        GameObject bullet = Instantiate(tornadoBulletGOPrefab, transform.position, tornadoBulletGOPrefab.transform.rotation);
        bullet.transform.position = new Vector3(bullet.transform.position.x, 0.5f, bullet.transform.position.z);
        bullet.GetComponent<Bullet>().Fire(target.transform);
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
