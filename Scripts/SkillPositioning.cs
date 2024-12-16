using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPositioning : MonoBehaviour
{
    private float skillRange;

    private void Start()
    {
        skillRange = GetComponent<CircleCollider2D>().radius * transform.localScale.x;
    }

    private void Update()
    {
        if (SkillManager.instance.isActivating)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            transform.position = mousePosition;
        }
    }

    public void Activate()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, skillRange);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (!enemyCollider.isTrigger && enemyCollider.CompareTag("Enemy"))
            {
                SkillManager.instance.ApplySkillEffectToEnemy(enemyCollider.gameObject);
            }
        }
        Destroy(gameObject);
    }
}
