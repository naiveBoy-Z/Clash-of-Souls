using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    UnitStats unitStats;
    AttackEnemy attackEnemy;
    public Animator animator;

    [HideInInspector] public Vector3[] waypoints;
    [HideInInspector] public List<GameObject> allNormalUnitsOnRoute = new();
    private int currentIndex = 0;
    private bool facingRight = true;

    void Start()
    {
        unitStats = GetComponent<UnitStats>();
        attackEnemy = GetComponent<AttackEnemy>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (attackEnemy.enemiesInRange.Count > 0)
        {
            if (attackEnemy.DetectTheClosestEnemy().transform.position.x < transform.position.x && facingRight ||
                attackEnemy.DetectTheClosestEnemy().transform.position.x > transform.position.x && !facingRight)
            {
                Flip();
            }
            animator.SetBool("isMoving", false);
            return;
        }
        animator.SetBool("isMoving", true);

        // If this unit is the first unit in list
        if (this.gameObject == allNormalUnitsOnRoute[0])
        {
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentIndex], unitStats.speed * Time.deltaTime);
        }
        // If this unit is NOT the first unit in list
        else
        {
            int unitPrevIndexInList = allNormalUnitsOnRoute.IndexOf(this.gameObject) - 1;

            if (Vector3.Distance(transform.position, allNormalUnitsOnRoute[unitPrevIndexInList].transform.position) <= 1.25f)
            {
                animator.SetBool("isMoving", false);
                return;
            }
            transform.position = Vector3.MoveTowards(transform.position, waypoints[currentIndex], unitStats.speed* Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, waypoints[currentIndex]) < 0.1f && currentIndex < waypoints.Length - 1)
        {
            currentIndex++;
        }
        if (currentIndex == waypoints.Length - 1 && Vector3.Distance(transform.position, waypoints[currentIndex]) < 0.1f)
        {
            animator.SetBool("isMoving", false);
        }

        if (transform.position.x - waypoints[currentIndex].x > 0.2f && facingRight ||
            transform.position.x - waypoints[currentIndex].x < -0.2f && !facingRight)
            Flip();
        
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    private void OnDestroy()
    {
        allNormalUnitsOnRoute.Remove(gameObject);
        StopAllCoroutines();
    }
}
