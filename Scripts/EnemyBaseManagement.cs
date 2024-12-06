using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBaseManagement : MonoBehaviour
{
    public static EnemyBaseManagement Instance { get; set; }

    [Header("Resource")]
    public int souls = 1000;
    public int coins = 1000;

    [Header("Health Bar")]
    public int maxHP = 1000;
    private int currentHP;
    public Image healthBar;

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
    }

    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateBaseHealthBar();

        if (currentHP == 0)
        {
            Victory();
        }
    }

    private void UpdateBaseHealthBar()
    {
        healthBar.fillAmount = (float) currentHP / maxHP;
    }

    private void Victory()
    {

    }
}
