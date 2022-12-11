using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using Sirenix.OdinInspector;

public class Enemy : MonoBehaviour
{
    private enum AttackType { DIRECT_ATTACK, IN_DIRECT_ATTACK};

    [SerializeField]
    private AttackType attackType;

    [HideIf("isSpeceficallyImplemented",true)]
    [ShowIf("attackType",AttackType.IN_DIRECT_ATTACK)]
    [SerializeField]
    private GameObject enemyBulletGO;

    [ShowIf("attackType", AttackType.IN_DIRECT_ATTACK)]
    [SerializeField]
    private float firingHeight;

    [SerializeField]
    private float rotationSpeed;

    [SerializeField]
    private float targetRange;

    [SerializeField]
    private float attackRate;

    [SerializeField]
    private bool lookAtPlayer;

    [SerializeField]
    private bool lookAtPlayerAroundYAxis;

    [ShowIf("attackType", AttackType.DIRECT_ATTACK)]
    [SerializeField]
    private int damage;

    [SerializeField]
    private int health;

    [SerializeField]
    private float healthBarHeightOffset;

    //[SerializeField]
    //private string attackAnimation, followAnimation, deathAnimation, idleAnimation, hitAnimation;

    [SerializeField]
    private bool isSpeceficallyImplemented;

    [ShowIf("isSpeceficallyImplemented", true)]
    [SerializeField]
    private SpecificEnemy specificEnemy;

    [SerializeField]
    private SkinnedMeshRenderer enemyMeshRenderer;

    [SerializeField]
    private float flashBrightness;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private EnemyRangeType rangeType;

    [SerializeField]
    private EnemyMovementType movementType;

    [SerializeField]
    private GameObject deathPoofEffect;

    [SerializeField]
    private Transform deathPoofPos;

    [SerializeField]
    private int coin;

    //[SerializeField]
    //private bool isRandomMovementEnemy;

    private AIPath aIPath;

    private AIDestinationSetter aIDestinationSetter;

    private Transform target;

    private EnemyGenerator enemyGenerator;

    private float attackTimer;

    private bool isAlive = true;

    private HealthBar healthBar;

    private GameObject pointer;

    private int shaderId;

    private float enemyFlashSpeed = 60f;

    private Coroutine flashCoroutine;

    private MaterialPropertyBlock materialPropertyBlock;

    private int IDLE_ANIMATION, MOVE_ANIMATION, ATTACK_ANIMATION, HIT_ANIMATION, DEAD_ANIMATION;

    private Player targetPlayer;

    private Rigidbody rb;

    private float fireLevel;

    void Start()
    {

        fireLevel = enemyGenerator.GetFireLevel();

        aIPath = GetComponent<AIPath>();
        rb = GetComponent<Rigidbody>();

        materialPropertyBlock = new MaterialPropertyBlock();

        shaderId = Shader.PropertyToID("_Glow");

        //Reseting glow effect if the value is set in previous game
        enemyMeshRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat(shaderId, 0);
        enemyMeshRenderer.SetPropertyBlock(materialPropertyBlock);

        //Set the health to the health bar
        healthBar.gameObject.SetActive(true);
        healthBar.SetTotalHealth(health, healthBarHeightOffset);

        //Animation string to hash
        IDLE_ANIMATION = Animator.StringToHash("Idle");
        MOVE_ANIMATION = Animator.StringToHash("Move");
        ATTACK_ANIMATION = Animator.StringToHash("Attack");
        HIT_ANIMATION = Animator.StringToHash("Hit");
        DEAD_ANIMATION = Animator.StringToHash("Dead");

        animator.SetTrigger(MOVE_ANIMATION);
    }

    void Update()
    {
        if (movementType.Equals(EnemyMovementType.RANDOM) || movementType.Equals(EnemyMovementType.STATIC))
        {
            return;
        }
        else if(movementType.Equals(EnemyMovementType.CLOSE_RANGE) || movementType.Equals(EnemyMovementType.MEDIUM_RANGE))
        {
            if (aIPath.reachedEndOfPath && isAlive)
            {
                aIPath.enabled = false;
                rb.isKinematic = true;
                if (lookAtPlayer && !lookAtPlayerAroundYAxis)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(-transform.position + target.position), Time.deltaTime * rotationSpeed);
                }
                else
                {
                    Vector3 pos = -transform.position + target.position;
                    pos.y = 0;
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(pos), Time.deltaTime * rotationSpeed);
                }
                Attack();
                animator.SetTrigger(IDLE_ANIMATION);
                if (Vector3.SqrMagnitude(transform.position - target.position) > (targetRange * targetRange))
                {
                    aIPath.enabled = true;
                    attackTimer = 0;
                    animator.SetTrigger(MOVE_ANIMATION);
                }
            }
            else if (isAlive)
            {
                animator.SetTrigger(MOVE_ANIMATION);
            }
        }
        else
        {
            //This is for long range enemies for which movement has to be specifically implemented, since it has to wander only around the created postion and doesn't have to follow player
            Attack();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isAlive && collision.gameObject.GetComponent<Bullet>() && !collision.gameObject.GetComponent<Bullet>().IsAreaAttack())
        {
            Hit(collision.gameObject.GetComponent<Bullet>().GetDamage());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (movementType.Equals(EnemyMovementType.RANDOM) && other.GetComponent<Player>())
        {
            other.GetComponent<Player>().Hit(damage);
        }
    }

    public void Hit(int _damage)
    {
        health -= (int)(_damage +(damage * fireLevel * 0.05f));
        healthBar.SetHealth(health);
        animator.SetTrigger(HIT_ANIMATION);
        //Start Flash Coroutine
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashEnemy());
        if (health <= 0)
        {
            enemyGenerator.UpdateCoins(coin);
            enemyGenerator.EnemyDestroyed(gameObject);
            aIPath.enabled = false;//This is disabled to stop moving the enemy if it is moving
            enemyGenerator.ResetPlayerTarget();
            isAlive = false;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
            gameObject.layer = 12;
            animator.SetTrigger(DEAD_ANIMATION);
            //Destroy the health bar when destroying the enemy
            Destroy(healthBar.gameObject);
            //Destroy the pointer for the enemy
            Destroy(pointer);
        }
    }

    private void Attack()
    {
        if (attackTimer > (1 / attackRate) && targetPlayer.IsAlive())
        {
            attackTimer = 0;
            if (!isSpeceficallyImplemented)
            {
                animator.SetTrigger(ATTACK_ANIMATION);
                if (attackType.Equals(AttackType.DIRECT_ATTACK))
                {
                    targetPlayer.Hit(damage);
                }
                else
                {
                    GameObject bullet = Instantiate(enemyBulletGO, transform.position, enemyBulletGO.transform.rotation);
                    bullet.transform.position = new Vector3(bullet.transform.position.x, firingHeight, bullet.transform.position.z);
                    bullet.GetComponent<Bullet>().Fire(target.transform);
                }
            }
            else
            {
                specificEnemy.Attack(target);
            }
        }
        attackTimer += Time.deltaTime;
    }

    public void SetEnemyGenerator(EnemyGenerator _enemyGenerator)
    {
        enemyGenerator = _enemyGenerator;
    }

    public void SetHealthBar(HealthBar _healthBar)
    {
        healthBar = _healthBar;
    }

    public void SetPointer(GameObject _pointer)
    {
        pointer = _pointer;
    }

    public void SetTargetPlayer(Player _targetPlayer)
    {
        target = _targetPlayer.transform;
        targetPlayer = _targetPlayer;
        if (!movementType.Equals(EnemyMovementType.LONG_RANGE) && !movementType.Equals(EnemyMovementType.RANDOM))
        {
            GetComponent<AIDestinationSetter>().target = _targetPlayer.transform;
        }
    }

    public EnemyRangeType GetRangeType()
    {
        return rangeType;
    }

    public int GetCoin()
    {
        return coin;
    }

    public HealthBar GetHealthBar()
    {
        return healthBar;
    }

    public Transform GetTarget()
    {
        return target;
    }

    public void DestroyEnemy()
    {
        Vector3 pos = transform.position;
        if(deathPoofPos)
        {
            pos = deathPoofPos.position;
        }
        pos.y = deathPoofEffect.transform.position.y;
        Instantiate(deathPoofEffect, pos, transform.rotation);
        enemyGenerator.CreateCoinBlastAt(pos);
        Destroy(gameObject,0.25f);
    }

    private IEnumerator FlashEnemy()
    {
        float temp = 0;

        while (temp < flashBrightness)
        {
            enemyMeshRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetFloat(shaderId, temp);
            enemyMeshRenderer.SetPropertyBlock(materialPropertyBlock);
            temp += enemyFlashSpeed * Time.deltaTime;
            yield return null;
        }
        while (temp > 0)
        {
            enemyMeshRenderer.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetFloat(shaderId, temp);
            enemyMeshRenderer.SetPropertyBlock(materialPropertyBlock);
            temp -= enemyFlashSpeed * Time.deltaTime;
            yield return null;
        }
        enemyMeshRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetFloat(shaderId, 0);
        enemyMeshRenderer.SetPropertyBlock(materialPropertyBlock);
    }
}

public class SpecificEnemy : MonoBehaviour
{
    public virtual void Attack(Transform target) { }
}

public enum EnemyRangeType { CLOSE_MEDIUM, MEDIUM, CLOSE, MEDIUM_LONG, LONG}

public enum EnemyMovementType { RANDOM, CLOSE_RANGE, MEDIUM_RANGE, LONG_RANGE, STATIC}
