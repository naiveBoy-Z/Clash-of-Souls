using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseManagement : MonoBehaviour
{
    public static BaseManagement Instance { get; set; }

    [Header("Resource")]
    public int souls = 1000;
    public TextMeshProUGUI soulText;
    public int coins = 1000;
    public TextMeshProUGUI coinText;

    [Header("Health Bar")]
    public int maxHP = 1000;
    private int currentHP;
    public Image healthBar;

    [Header("Defeat Panel")]
    public GameObject defeatPanel;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        currentHP = maxHP;
        defeatPanel.SetActive(false);
    }

    void Update()
    {
        soulText.text = souls + "";
        coinText.text = coins + "";
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateBaseHealthBar();

        if (currentHP == 0)
        {
            Defeat();
        }
    }

    private void UpdateBaseHealthBar()
    {
        healthBar.fillAmount = (float)currentHP / maxHP;
    }

    private void Defeat()
    {
        defeatPanel.SetActive(true);

        Time.timeScale = 0f;
    }
}
