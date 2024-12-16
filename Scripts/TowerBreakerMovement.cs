using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBreakerMovement : MonoBehaviour
{
    public EnemyStats enemyStats;
    public EnemyHealth enemyHealth;
    public TowerBreakerAttackTower towerBreakerAttackTower;
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    public Vector3[] waypoints;
    private int currentIndex = 0;
    private bool facingLeft = true;

    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        enemyHealth = GetComponent<EnemyHealth>();
        towerBreakerAttackTower = GetComponent<TowerBreakerAttackTower>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (towerBreakerAttackTower.targetTower != null)
        {
            if (towerBreakerAttackTower.targetTower.transform.position.x < transform.position.x && !facingLeft ||
                towerBreakerAttackTower.targetTower.transform.position.x > transform.position.x && facingLeft)
            {
                Flip();
            }
            animator.SetBool("isMoving", false);
            return;
        }
        if (enemyHealth.currentHP == 0 || enemyStats.isAttacking || enemyStats.isFreeze) return;
        animator.SetBool("isMoving", true);

        // move the tower breaker
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentIndex], enemyStats.speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, waypoints[currentIndex]) < 0.1f && currentIndex < waypoints.Length - 1)
        {
            currentIndex++;
        }
        if (currentIndex == waypoints.Length - 1 && Vector3.Distance(transform.position, waypoints[currentIndex]) < 0.1f)
        {
            animator.SetBool("isMoving", false);
        }

        // flip the tower breaker sprite
        if (transform.position.x - waypoints[currentIndex].x > 0.1f && !facingLeft || transform.position.x - waypoints[currentIndex].x < -0.1f && facingLeft) Flip();
    }

    private void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    public void Freeze(int damage, float duration)
    {
        StartCoroutine(FreezeEnemy(damage, duration));
    }

    private IEnumerator FreezeEnemy(int damage, float duration)
    {
        enemyHealth.originalColor = Color.blue;
        spriteRenderer.color = Color.blue;
        enemyStats.isFreeze = true;

        yield return new WaitForSeconds(duration);

        Unfreeze();
        enemyHealth.TakeDamage(damage);
    }

    public void Unfreeze()
    {
        enemyHealth.originalColor = Color.white;
        enemyStats.isFreeze = false;
        GetComponent<Animator>().speed = 1;
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
    }
}
