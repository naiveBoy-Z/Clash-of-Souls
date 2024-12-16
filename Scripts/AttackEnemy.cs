using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackEnemy : MonoBehaviour
{
    public UnitStats unitStats;
    public CircleCollider2D circleCollider;
    public Animator animator;

    public List<GameObject> enemiesInRange = new();
    public GameObject closetEnemy;
    public bool readyToAttackBase = true;
    public Coroutine attackEnemiesCoroutine;
    public Coroutine attackBaseCoroutine;

    // create 2 events when the enemy detects unit and base
    public event Action OnUnitDetected;
    public event Action OnBaseDetected;

    void Start()
    {
        unitStats = GetComponent<UnitStats>();
        circleCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        OnUnitDetected += AttackEnemyInRange;
        OnBaseDetected += AttackBase;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger && other.CompareTag("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
            OnUnitDetected?.Invoke();
        }
        else if (other.CompareTag("EnemyBase"))
        {
            OnBaseDetected?.Invoke();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.isTrigger && other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
            if (other.gameObject == closetEnemy) closetEnemy = null;
        }
        else if (other.CompareTag("EnemyBase"))
        {
            readyToAttackBase = false;
        }
    }

    public void AttackEnemyInRange()
    {
        readyToAttackBase = false;
        attackEnemiesCoroutine ??= StartCoroutine(AimToClosetEnemy());
    }

    public IEnumerator AimToClosetEnemy()
    {
        while (enemiesInRange.Count > 0)
        {
            closetEnemy = DetectTheClosestEnemy();

            // delay before attacking the closest enemy:
            yield return new WaitForSeconds(unitStats.attackDelay);

            // attack until the enemy dies or moves out of this unit's range:
            while (closetEnemy != null)
            {
                animator.SetBool("isAttacking", true);
                yield return new WaitForSeconds(unitStats.dealingDamageMomoment);

                if (closetEnemy != null)
                Attack(closetEnemy);

                yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - unitStats.dealingDamageMomoment);
                animator.SetBool("isAttacking", false);

                yield return new WaitForSeconds(unitStats.attackCD);
            }
        }

        // the unit will be ready to attack the base if there are no enemies in range
        if (enemiesInRange.Count == 0) readyToAttackBase = true;

        attackEnemiesCoroutine = null;
    }

    public GameObject DetectTheClosestEnemy()
    {
        GameObject closestEnemy = null;
        if (enemiesInRange.Count == 1) closestEnemy = enemiesInRange[0];
        else
        {
            float closestDistance = Mathf.Infinity;

            foreach (GameObject enemy in enemiesInRange)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }

    private void OnDestroy()
    {
        // remove event subscriptions
        OnUnitDetected = null;
        OnBaseDetected = null;
    }

    public abstract void Attack(GameObject target);
    public abstract void AttackBase();
}