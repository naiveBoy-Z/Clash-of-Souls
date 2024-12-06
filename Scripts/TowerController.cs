using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TowerController : MonoBehaviour
{
    public Image hpAmountInRepairSelection;

    [Header("Tower Stats")]
    public int price = 1000;
    public int damage = 5;
    public int hp = 100;
    public float range = 7;
    public float attackCooldown = 2;
    public float attackDelay = 1;
    public int maintenanceEnergyPerSecond = 1;
    [HideInInspector] public int currentHP;
    [HideInInspector] public int repairCost;
    public RectTransform energyPanel;
    public TextMeshProUGUI energyText;

    [Header("Tower Range")]
    public int numberOfPoints = 50;
    private CircleCollider2D circleCollider;
    private LineRenderer lineRenderer;

    [Header("Tower Upgrade")]
    public int currentLv = 1;
    public int upgradeCostToLv2 = 1500;
    public int upgradeCostToLv3 = 2000;

    [Header("Attack Details")]
    public GameObject projectileType;
    public List<GameObject> enemiesInRange = new();
    private Coroutine attackEnemyCoroutine;

    [Header("Buff Details")]
    public List<GameObject> meleeUnitsInRange = new();
    public List<GameObject> rangedUnitsInRange = new();

    [Header("Build Spot")]
    public GameObject buildSpot;

    // create an event when the tower detects enemies
    public event Action OnEnemiesDetected;

    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        lineRenderer = GetComponent<LineRenderer>();

        circleCollider.radius = range;
        currentHP = hp;
        energyPanel.gameObject.SetActive(false);
        energyText.text = " -" + maintenanceEnergyPerSecond.ToString();

        StartCoroutine(DecreaseSoulsCoroutine());

        DrawTowerRange(1);
        lineRenderer.enabled = false;

        OnEnemiesDetected += AttackEnemy;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.isTrigger)
        {
            if (other.CompareTag("Enemy"))
            {
                enemiesInRange.Add(other.gameObject);
                OnEnemiesDetected?.Invoke();
            }

            // else if: archer unit is added to 'meleeUnitInRange' list
            else if (other.CompareTag("Unit") && other.GetComponent<UnitStats>().type == 1)
            {
                meleeUnitsInRange.Add(other.gameObject);
                if (this.name == "Military Tent(Clone)") EnhanceMeleeUnit(other.gameObject);
            }

            // else if: archer unit is added to 'rangedUnitInRange' list
            else if (other.CompareTag("Unit") && other.GetComponent<UnitStats>().type == 2)
            {
                rangedUnitsInRange.Add(other.gameObject);
                if (this.name == "Lookout Tower(Clone)") EnhanceRangedUnit(other.gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.isTrigger)
        {
            if (other.CompareTag("Enemy"))
            {
                enemiesInRange.Remove(other.gameObject);
            }

            // else if: melee unit is removed to 'meleeUnitInRange' list
            else if (other.CompareTag("Unit") && other.GetComponent<UnitStats>().type == 1 && this.name == "Military Tent(Clone)")
            {
                meleeUnitsInRange.Remove(other.gameObject);
                NormalizeMeleeUnit(other.gameObject);
            }

            // else if: ranged unit is removed to 'rangedUnitInRange' list
            else if (other.CompareTag("Unit") && other.GetComponent<UnitStats>().type == 2 && this.name == "Lookout Tower(Clone)")
            {
                rangedUnitsInRange.Remove(other.gameObject);
                NormalizeRangedUnit(other.gameObject);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, hp);
        UpdateHealthBar();

        if (currentHP == 0)
        {
            TowerFallen();
        }
        repairCost = Mathf.RoundToInt((1 - (float)currentHP / hp) * 0.75f * price);
    }

    public void UpdateHealthBar()
    {
        hpAmountInRepairSelection.fillAmount = (float)currentHP / hp;
    }

    private void TowerFallen()
    {
        SpotManager.Instance.SellTower(this);
    }

    private void AttackEnemy()
    {
        attackEnemyCoroutine ??= StartCoroutine(AimToClosestEnemy());
    }

    public IEnumerator AimToClosestEnemy()
    {
        while (enemiesInRange.Count > 0)
        {
            GameObject closetEnemy = DetectTheClosestEnemy();

            // delay before attacking the closest enemy:
            yield return new WaitForSeconds(attackDelay);

            // attack until the enemy dies or moves out of this tower's range:
            while (closetEnemy != null && Vector3.Distance(transform.position, closetEnemy.transform.position) <= circleCollider.radius + 0.5f)
            {
                Attack(closetEnemy);
                yield return new WaitForSeconds(attackCooldown);
            }
        }
    }

    public GameObject DetectTheClosestEnemy()
    {
        GameObject closestEnemy = null;
        if (enemiesInRange.Count == 1) closestEnemy = enemiesInRange[0];
        else
        {
            float closestDistance = Mathf.Infinity;

            foreach (GameObject enemy in enemiesInRange)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
        }

        return closestEnemy;
    }

    private void Attack(GameObject target)
    {
        Vector3 spawnPosition = transform.position;

        Vector2 direction = (target.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

        GameObject attackingProjectile = Instantiate(projectileType, spawnPosition, spawnRotation);
        attackingProjectile.GetComponent<Projectile>().target = target;
        attackingProjectile.GetComponent <Projectile>().dmg = damage;
    }

    private void EnhanceRangedUnit(GameObject unit)
    {
        // enhance the attack range; decrease the attack cooldown
        unit.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        unit.GetComponent<UnitHealth>().originalColor = Color.yellow;
        unit.GetComponent<SpriteRenderer>().color = Color.yellow;

        unit.GetComponent<CircleCollider2D>().radius = 5.5f;
        unit.GetComponent<UnitStats>().attackCD = 0.75f;
    }

    private void NormalizeRangedUnit(GameObject unit)
    {
        unit.transform.localScale = Vector3.one;
        unit.GetComponent<UnitHealth>().originalColor = Color.white;
        unit.GetComponent<SpriteRenderer>().color = Color.white;

        unit.GetComponent<CircleCollider2D>().radius = 4.2f;
        unit.GetComponent<UnitStats>().attackCD = 1;
    }

    private void EnhanceMeleeUnit(GameObject unit)
    {
        // enhance the attack range slighly, damage, hp; increase the attack cooldown slightly;
        unit.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        unit.GetComponent<UnitHealth>().originalColor = Color.yellow;
        unit.GetComponent<SpriteRenderer>().color = Color.yellow;

        UnitStats unitStats = unit.GetComponent<UnitStats>();
        UnitHealth unitHealth = unit.GetComponent<UnitHealth>();
        unitStats.damage = 30;
        unitStats.hp = 120;
        unitStats.attackCD = 2;
        unitHealth.currentHP = unitStats.hp;
        unit.GetComponent<CircleCollider2D>().radius = 1.2f;
    }

    private void NormalizeMeleeUnit(GameObject unit)
    {
        unit.transform.localScale = Vector3.one;
        unit.GetComponent<UnitHealth>().originalColor = Color.white;
        unit.GetComponent<SpriteRenderer>().color = Color.white;

        UnitStats unitStats = unit.GetComponent<UnitStats>();
        UnitHealth unitHealth = unit.GetComponent<UnitHealth>();
        unitStats.damage = 10;
        unitStats.hp = 100;
        unitStats.attackCD = 1;
        unitHealth.currentHP = Mathf.Clamp(unitHealth.currentHP, 0, unitStats.hp); ;
        unit.GetComponent<CircleCollider2D>().radius = 1;
    }

    private IEnumerator DecreaseSoulsCoroutine()
    {
        yield return new WaitForSeconds(5);
        while (BaseManagement.Instance.souls >= maintenanceEnergyPerSecond)
        {
            Coroutine fleeingSoulCoroutine = StartCoroutine(FleeingSoul());
            BaseManagement.Instance.souls -= maintenanceEnergyPerSecond;
            yield return new WaitForSeconds(2);
            StopCoroutine(fleeingSoulCoroutine);
        }
    }

    private IEnumerator FleeingSoul()
    {
        energyPanel.gameObject.SetActive(true);
        float elapsedTime = 0f;
        Vector2 startPosition = new(0, 2.7f);
        Vector2 endPosition = new(0, 3.2f);

        while (elapsedTime < 1.5f)
        {
            energyPanel.anchoredPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / 1.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        energyPanel.gameObject.SetActive(false);
    }

    public void DrawTowerRange(int numberOfRange)
    {
        if (numberOfRange == 1)
        {
            ConfigureLineRenderer(lineRenderer, Color.green, numberOfPoints);

            float angleStep = 360f / numberOfPoints;
            Vector3[] points = new Vector3[numberOfPoints + 1];

            for (int i = 0; i <= numberOfPoints; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * circleCollider.radius;
                float y = Mathf.Sin(angle) * circleCollider.radius;
                points[i] = new Vector3(x, y, 0);
            }

            lineRenderer.SetPositions(points);
        }

        else if (numberOfRange == 2) {
            ConfigureLineRenderer(lineRenderer, Color.green, numberOfPoints * 2 + 1);

            float angleStep = 360f / numberOfPoints;
            Vector3[] points = new Vector3[numberOfPoints * 2 + 2];

            for (int i = 0; i <= numberOfPoints; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * circleCollider.radius;
                float y = Mathf.Sin(angle) * circleCollider.radius;
                points[i] = new Vector3(x, y, 0);
            }

            for (int i = numberOfPoints; i <= numberOfPoints * 2; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * (circleCollider.radius + 0.5f);
                float y = Mathf.Sin(angle) * (circleCollider.radius + 0.5f);
                points[i + 1] = new Vector3(x, y, 0);
            }

            lineRenderer.SetPositions(points);
        }
    }

    void ConfigureLineRenderer(LineRenderer lr, Color color, int points)
    {
        lr.positionCount = points + 1;
        lr.widthMultiplier = 0.05f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.endColor = color;
        lr.useWorldSpace = false;
    }

    private void OnDestroy()
    {
        Instantiate(buildSpot, transform.position, Quaternion.identity);

        StopAllCoroutines();

        // remove the event subscription
        OnEnemiesDetected = null;
    }
}
