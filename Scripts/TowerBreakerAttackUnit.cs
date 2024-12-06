using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBreakerAttackUnit : MonoBehaviour
{
    public EnemyStats enemyStats;
    public TowerBreakerAttackTower towerBreakerAttackTower;
    public Animator animator;

    public List<GameObject> unitsInRange = new();
    public bool readyToAttackBase = true;
    public Coroutine attackUnitsCoroutine;
    public Coroutine attackBaseCoroutine;

    // create 2 events when the enemy detects units and base
    public event Action OnUnitsDetected;
    public event Action OnBaseDetected;

    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        towerBreakerAttackTower = GetComponent<TowerBreakerAttackTower>();
        animator = GetComponent<Animator>();
        OnUnitsDetected += AttackAllUnitsInRange;
        OnBaseDetected += AttackBase;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.CompareTag("Unit"))
        {
            unitsInRange.Add(collision.gameObject);
            OnUnitsDetected?.Invoke();
        }
        else if (collision.CompareTag("MyBase"))
        {
            OnBaseDetected?.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.CompareTag("Unit"))
        {
            unitsInRange.Remove(collision.gameObject);
        }
        else if (collision.CompareTag("MyBase"))
        {
            readyToAttackBase = false;
        }
    }

    public void AttackAllUnitsInRange()
    {
        readyToAttackBase = false;
        attackUnitsCoroutine ??= StartCoroutine(AttackAllUnitsInRangeCoroutine());
    }

    public IEnumerator AttackAllUnitsInRangeCoroutine()
    {
        // delay before attacking units
        yield return new WaitForSeconds(enemyStats.attackDelay);

        // attack all units in range
        while (unitsInRange.Count > 0)
        {
            // do NOT attack while being frozen or while attacking a tower
            if (enemyStats.isFreeze || towerBreakerAttackTower.isAttackingTower)
            {
                yield return null;
                continue;
            }


            // attack
            enemyStats.isAttacking = true;
            animator.SetBool("isAttacking", true);
            yield return new WaitForSeconds(0.25f);

            List<GameObject> unitsToAttack = new(unitsInRange);
            foreach (GameObject unit in unitsToAttack)
            {
                UnitHealth unitHealth = unit.GetComponent<UnitHealth>();
                unitHealth.TakeDamage(enemyStats.damage);
            }

            yield return new WaitForSeconds(0.75f);
            enemyStats.isAttacking = false;
            animator.SetBool("isAttacking", false);

            yield return new WaitForSeconds(enemyStats.attackCD);
        }
        attackUnitsCoroutine = null;

        // the enemy will be ready to attack the base if there are no units in range
        if (unitsInRange.Count == 0) readyToAttackBase = true;
    }


    public void AttackBase()
    {
        attackBaseCoroutine = StartCoroutine(AttackBaseCoroutine());
    }

    public IEnumerator AttackBaseCoroutine()
    {
        // delay before attacking units
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
            yield return new WaitForSeconds(0.25f);

            BaseManagement.Instance.TakeDamage(enemyStats.damage * 5);

            yield return new WaitForSeconds(0.75f);
            enemyStats.isAttacking = false;
            animator.SetBool("isAttacking", false);

            yield return new WaitForSeconds(enemyStats.attackCD - 0.25f);
        }
    }
    private void OnDestroy()
    {
        // remove event subscriptions
        OnUnitsDetected = null;
        OnBaseDetected = null;
    }
}
