using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [HideInInspector] public EnemyStats enemyStats;
    [HideInInspector] public EnemyHealth enemyHealth;
    [HideInInspector] public EnemyMeleeAttackUnit enemyMeleeAttackUnit;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Animator animator;

    [HideInInspector] public Vector3[] waypoints;
    [HideInInspector] public List<GameObject> allNormalEnemiesOnRoute = new();
    private int currentIndex = 0;
    private bool facingLeft = true;

    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyMeleeAttackUnit = GetComponent<EnemyMeleeAttackUnit>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (enemyMeleeAttackUnit.targetUnit != null)
        {
            if (enemyMeleeAttackUnit.targetUnit.transform.position.x < transform.position.x && !facingLeft ||
                enemyMeleeAttackUnit.targetUnit.transform.position.x > transform.position.x && facingLeft)
            {
                Flip();
            }
            animator.SetBool("isMoving", false);
            return;
        }
        if (enemyStats.isFreeze || enemyHealth.currentHP == 0) return;
        animator.SetBool("isMoving", true);

        // move the enemies if they are NOT attacking or NOT frozen
        // if this unit is the first enemy on the route:
        if (this.gameObject == allNormalEnemiesOnRoute[0])
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentIndex], enemyStats.speed * Time.deltaTime);
        }
        // If this unit is NOT the first unit in list
        else
        {
            int unitPrevIndexInList = allNormalEnemiesOnRoute.IndexOf(this.gameObject) - 1;

            if (Vector3.Distance(transform.position, allNormalEnemiesOnRoute[unitPrevIndexInList].transform.position) <= 1.25f)
            {
                animator.SetBool("isMoving", false);
                return;
            }
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentIndex], enemyStats.speed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, waypoints[currentIndex]) < 0.1f && currentIndex < waypoints.Length - 1)
        {
            currentIndex++;
        }
        if (currentIndex == waypoints.Length - 1 && Vector3.Distance(transform.position, waypoints[currentIndex]) < 0.1f)
        {
            animator.SetBool("isMoving", false);
        }

        // flip the enemy's sprite
        if (transform.position.x - waypoints[currentIndex].x > 0.1f && !facingLeft ||
            transform.position.x - waypoints[currentIndex].x < -0.1f && facingLeft) Flip();
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
        allNormalEnemiesOnRoute.Remove(gameObject);
        StopAllCoroutines();
    }
}
