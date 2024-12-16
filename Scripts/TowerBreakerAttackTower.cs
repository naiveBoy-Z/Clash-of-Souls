using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBreakerAttackTower : MonoBehaviour
{
    public EnemyStats enemyStats;
    public Animator animator;

    public GameObject targetTower;
    public Coroutine attackTowerCoroutine;
    public bool isAttackingTower = false;

    // create an event when the enemy detects a tower
    public event Action OnTowerDetected;

    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        animator = GetComponent<Animator>();
        OnTowerDetected += AttackTower;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.isTrigger && collision.CompareTag("Tower"))
        {
            targetTower = collision.gameObject;
            OnTowerDetected?.Invoke();
        }
    }

    public void AttackTower()
    {
        isAttackingTower = true;
        TowerController tower = targetTower.GetComponent<TowerController>();
        attackTowerCoroutine = StartCoroutine(AttackTowerCoroutine(tower));
    }

    public IEnumerator AttackTowerCoroutine(TowerController tower)
    {
        // delay before attacking the tower
        yield return new WaitForSeconds(enemyStats.attackDelay);


        // attack the tower until it is destroyed
        while (targetTower != null)
        {
            while (enemyStats.isFreeze)
            {
                yield return null;
            }

            if (tower == null) break;

            animator.SetBool("isAttacking", true);
            yield return new WaitForSeconds(0.25f);

            if (tower != null)
            {
                tower.TakeDamage(enemyStats.damage * 10);
            }

            yield return new WaitForSeconds(0.75f);
            animator.SetBool("isAttacking", false);

            yield return new WaitForSeconds(enemyStats.attackCD - 1);
        }

        // after destroy the tower
        isAttackingTower = false;
        attackTowerCoroutine = null;
    }

    private void OnDestroy()
    {
        // remove event subscription
        OnTowerDetected = null;
    }
}
