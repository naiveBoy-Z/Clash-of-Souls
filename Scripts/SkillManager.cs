using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public List<GameObject> allSkillButton = new();
    public List<GameObject> allSkillIndicator = new();
    [HideInInspector] public int selectedSkillIndex;

    private GameObject skillIndicator;
    private float skillRange = 0;

    [Header("Fire Skill Stats")]
    public float fireCooldown = 20;
    public int burnDamageInstant = 20;
    public int burnDamageOverTime = 5;
    public float burnDuration = 3f;
    public float burnInterval = 0.5f;
    public Image fireCooldownOverlay;
    [HideInInspector] public float fireCooldownTimer = 0;

    [Header("Ice Skill Stats")]
    public int iceCooldown = 20;
    public int freezeDamage = 10;
    public float freezeDuration = 5;
    public Image iceCooldownOverlay;
    [HideInInspector] public float iceCooldownTimer = 0;

    private void Start()
    {
        fireCooldownOverlay.fillAmount = fireCooldownTimer / fireCooldown;
        iceCooldownOverlay.fillAmount = iceCooldownTimer / iceCooldown;
    }

    private void Update()
    {
        if (fireCooldownTimer > 0)
        {
            fireCooldownTimer -= Time.deltaTime;
            fireCooldownOverlay.fillAmount = fireCooldownTimer / fireCooldown;
        }
        if (iceCooldownTimer > 0)
        {
            iceCooldownTimer -= Time.deltaTime;
            iceCooldownOverlay.fillAmount = iceCooldownTimer / iceCooldown;
        }

        if (skillRange > 0 && Input.GetMouseButtonDown(0)) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(mousePosition, skillRange);

            foreach (Collider2D enemyCollider in hitEnemies)
            {
                if (!enemyCollider.isTrigger && enemyCollider.CompareTag("Enemy"))
                {
                    ApplySkillEffectToEnemy(selectedSkillIndex, enemyCollider.gameObject);
                }
            }
            Destroy(skillIndicator);
            skillRange = 0;
        }
    }

    public void SelectSkill(int skillButtonIndex)
    {
        if (skillButtonIndex == 0 && fireCooldownTimer <= 0 || skillButtonIndex == 1 && iceCooldownTimer <= 0)
        {
            selectedSkillIndex = skillButtonIndex;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            skillIndicator = Instantiate(allSkillIndicator[skillButtonIndex], mousePosition, Quaternion.identity);
            skillRange = skillIndicator.GetComponent<CircleCollider2D>().radius * skillIndicator.transform.localScale.x;
        }
    }

    private void ApplySkillEffectToEnemy(int skillIndex, GameObject enemy)
    {
        if (skillIndex == 0) ApplyFireSkillEffectToEnemy(enemy);
        else ApplyIceSkillEffectToEnemy(enemy);
    }

    private void ApplyFireSkillEffectToEnemy(GameObject enemy)
    {
        fireCooldownTimer = fireCooldown;

        // Unfreeze enemies if they are frozen
        if (enemy.GetComponent<EnemyStats>().isFreeze)
        {
            if (enemy.GetComponent<EnemyMovement>() != null)
                enemy.GetComponent<EnemyMovement>().Unfreeze();
            else enemy.GetComponent<TowerBreakerMovement>().Unfreeze();
        }

        // Start burning enemies
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        enemyHealth.TakeDamage(burnDamageInstant);
        if (enemy != null) StartCoroutine(DealBurnDamageOverTime(enemy));
    }

    private void ApplyIceSkillEffectToEnemy(GameObject enemy)
    {
        iceCooldownTimer = iceCooldown;

        if (enemy.GetComponent<EnemyMovement>() != null) 
        enemy.GetComponent<EnemyMovement>().Freeze(freezeDamage, freezeDuration);
        else enemy.GetComponent<TowerBreakerMovement>().Freeze(freezeDamage, freezeDuration);

        enemy.GetComponent<Animator>().speed = 0;
    }

    private IEnumerator DealBurnDamageOverTime(GameObject enemy)
    {
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

        for (float timeElapsed = 0f; timeElapsed < burnDuration; timeElapsed += burnInterval)
        {
            yield return new WaitForSeconds(burnInterval);
            if (enemy == null) yield break;
            enemyHealth.TakeDamage(burnDamageOverTime);
        }
    }
}
