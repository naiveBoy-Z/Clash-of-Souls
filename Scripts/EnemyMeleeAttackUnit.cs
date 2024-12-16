using System;
using System.Collections;
using UnityEngine;

public class EnemyMeleeAttackUnit : MonoBehaviour
{
    public EnemyStats enemyStats;
    public Animator animator;

    public GameObject targetUnit;
    public bool readyToAttackBase = true;
    public Coroutine attackUnitsCoroutine;
    public Coroutine attackBaseCoroutine;

    // create 2 events when the enemy detects unit and base
    public event Action OnUnitDetected;
    public event Action OnBaseDetected;

    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        animator = GetComponent<Animator>();
        OnUnitDetected += AttackTargetUnit;
        OnBaseDetected += AttackBase;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger && other.CompareTag("Unit"))
        {
            targetUnit = other.gameObject;
            OnUnitDetected?.Invoke();
        }
        else if (other.CompareTag("MyBase"))
        {
            OnBaseDetected?.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.CompareTag("Unit"))
        {
            targetUnit = null;
        }
        else if (collision.CompareTag("MyBase"))
        {
            readyToAttackBase = false;
        }
    }

    public void AttackTargetUnit()
    {
        readyToAttackBase = false;
        attackUnitsCoroutine ??= StartCoroutine(AttackUnit());
    }

    public IEnumerator AttackUnit()
    {
        // delay before attacking the unit
        yield return new WaitForSeconds(enemyStats.attackDelay);

        // attack the unit until it dies
        while (targetUnit != null)
        {
            // do NOT attack while being frozen
            while (enemyStats.isFreeze)
            {
                yield return null;
            }

            if (targetUnit == null) break;

            // attack
            animator.SetBool("isAttacking", true);
            yield return new WaitForSeconds(enemyStats.dealingDamageMoment);

            if (targetUnit != null)
            {
                UnitHealth unitHealth= targetUnit.GetComponent<UnitHealth>();
                unitHealth.TakeDamage(enemyStats.damage);
            }

            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - enemyStats.dealingDamageMoment);
            animator.SetBool("isAttacking", false);

            yield return new WaitForSeconds(enemyStats.attackCD);
        }

        // the enemy will be ready to attack the base if there are no units in range
        if (targetUnit == null) readyToAttackBase = true;

        attackUnitsCoroutine = null;
    }


    public void AttackBase()
    {
        attackBaseCoroutine = StartCoroutine(AttackBaseCoroutine());
    }

    public IEnumerator AttackBaseCoroutine()
    {
        // delay before attacking the ally base
        yield return new WaitForSeconds(enemyStats.attackDelay);

        // attack the ally base until it is destroyed
        while (BaseManagement.Instance.maxHP > 0)
        {
            // do NOT attack if not ready to attack the base
            while (!readyToAttackBase)
            {
                yield return null;
            }


            // attack
            enemyStats.isAttacking = true;
            animator.SetBool("isAttacking", true);
            yield return new WaitForSeconds(enemyStats.dealingDamageMoment);

            BaseManagement.Instance.TakeDamage(enemyStats.damage);

            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - enemyStats.dealingDamageMoment);
            animator.SetBool("isAttacking", false);

            yield return new WaitForSeconds(enemyStats.attackCD - 0.25f);
        }
    }
    private void OnDestroy()
    {
        // remove event subscriptions
        OnUnitDetected = null;
        OnBaseDetected = null;
    }
}
