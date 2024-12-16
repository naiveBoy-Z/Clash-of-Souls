using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public List<GameObject> allSkillButton = new();
    public List<GameObject> allSkillPrefabs = new();
    [HideInInspector] public int selectedSkillIndex;

    private GameObject skillIndicator;
    [HideInInspector] public bool isActivating = false;
    private Animator skillAnimator;

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

    
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(instance);
    }

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

        if (isActivating && Input.GetMouseButtonDown(0)) {
            skillAnimator.SetBool("isActivated", true);
            isActivating = false;
            if (selectedSkillIndex == 0) fireCooldownTimer = fireCooldown;
            else iceCooldownTimer = iceCooldown;
        }
    }

    public void SelectSkill(int skillButtonIndex)
    {
        if (skillButtonIndex == 0 && fireCooldownTimer <= 0 || skillButtonIndex == 1 && iceCooldownTimer <= 0)
        {
            selectedSkillIndex = skillButtonIndex;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            skillIndicator = Instantiate(allSkillPrefabs[skillButtonIndex], mousePosition, Quaternion.identity);
            skillAnimator = skillIndicator.GetComponent<Animator>();
            isActivating = true;
        }
    }

    public void ApplySkillEffectToEnemy(GameObject enemy)
    {
        if (selectedSkillIndex == 0) ApplyFireSkillEffectToEnemy(enemy);
        else ApplyIceSkillEffectToEnemy(enemy);
    }

    private void ApplyFireSkillEffectToEnemy(GameObject enemy)
    {
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
