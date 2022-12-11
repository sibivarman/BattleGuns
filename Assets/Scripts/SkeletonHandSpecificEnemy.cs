using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonHandSpecificEnemy : SpecificEnemy
{

    [SerializeField]
    private int damage;

    [SerializeField]
    private Animator animator;

    private bool canAttack;

    private Player player;

    private int attackAnimationHash = Animator.StringToHash("Attack");

    private void Start()
    {
        GetComponent<Enemy>().GetHealthBar().HideHealthBar();
    }

    public override void Attack(Transform target)
    {
        //base.Attack();
        if (canAttack)
        {
            animator.SetTrigger(attackAnimationHash);
            player.Hit(damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            canAttack = true;
            player = other.GetComponent<Player>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            canAttack = false;
        }
    }
}
