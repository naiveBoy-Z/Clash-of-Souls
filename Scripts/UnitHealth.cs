using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitHealth : MonoBehaviour
{
    public UnitStats unitStats;
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
        unitStats = GetComponent<UnitStats>();
        animator = GetComponent<Animator>();

        currentHP = unitStats.hp;
        UpdateHealthBar();
        loot.gameObject.SetActive(false);
        lootText.text = "+" + unitStats.fleeingSouls;

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
        hpBarForeground.fillAmount = (float)currentHP / unitStats.hp;
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

    public void Die()
    {
        EnemyBaseManagement.Instance.souls += unitStats.fleeingSouls;
        Destroy(gameObject);
    }
}
