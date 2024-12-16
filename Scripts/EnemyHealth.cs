using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public EnemyStats enemyStats;
    public Animator animator;

    public int currentHP;
    public Image hpBarForeground;
    public RectTransform loot;
    public TextMeshProUGUI lootText;

    private Coroutine takeDamageEffectCoroutine;
    [HideInInspector] public SpriteRenderer spriteRenderer;
    [HideInInspector] public Color originalColor;

    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        animator = GetComponent<Animator>();

        currentHP = enemyStats.hp;
        UpdateHealthBar();
        loot.gameObject.SetActive(false);
        lootText.text = "+" + enemyStats.fleeingSouls;

        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage)
    {
        // decrease hp
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            animator.SetBool("isDie", true);
            StartCoroutine(FleeingSoul());
        }
        UpdateHealthBar();

        // apply take damage effect
        if (takeDamageEffectCoroutine != null)
        {
            StopCoroutine(takeDamageEffectCoroutine);
        }

        takeDamageEffectCoroutine = StartCoroutine(ChangeColorOnDamage());
    }

    private IEnumerator ChangeColorOnDamage()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.15f);

        spriteRenderer.color = originalColor;
    }

    private void UpdateHealthBar()
    {
        hpBarForeground.fillAmount = (float)currentHP / enemyStats.hp;
    }

    public IEnumerator FleeingSoul()
    {
        loot.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Vector2 startPosition = loot.anchoredPosition;
        Vector2 endPosition = loot.anchoredPosition + new Vector2(0, 2);

        while (elapsedTime < 2)
        {
            loot.anchoredPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / 2);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void Die()
    {
        BaseManagement.Instance.souls += enemyStats.fleeingSouls;
        Destroy(gameObject);
    }
}
