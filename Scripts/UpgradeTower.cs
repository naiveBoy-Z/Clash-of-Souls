using TMPro;
using UnityEngine;

public class UpgradeTower : MonoBehaviour
{
    private TowerController towerController;
    // used to access the selected tower's current lv, upgradeCost, and all the tower stats
    public TowerDetail towerDetail;

    public LayerMask layerMask;
    public GameObject upgradeIcon;
    public GameObject upgradeCostTextObject;
    public TextMeshProUGUI upgradeCostText;
    public bool isManipulating = false;
    public bool isMaxLv = false;
    public TextMeshProUGUI levelIndicator;


    void Start()
    {
        towerController = transform.parent.parent.GetComponent<TowerController>();
        layerMask = LayerMask.GetMask("Tower Selection");
        upgradeCostText.text = towerController.upgradeCostToLv2.ToString();
    }

    void Update()
    {
        if (!isMaxLv && Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);
            if (hit.collider != null && hit.collider.gameObject.name == "Upgrade Selection")
            {
                if (!isManipulating) isManipulating = true;
                else
                {
                    Upgrade();
                }
            }
        }
    }

    public void DisplayUpgradeCost()
    {
        upgradeIcon.SetActive(false);
        upgradeCostTextObject.SetActive(true);
    }

    private void OnDisable()
    {
        isManipulating = false;

        if (!isMaxLv)
        {
            upgradeIcon.SetActive(true);
            upgradeCostTextObject.SetActive(false);
        }
    }

    public void Upgrade()
    {
        TowerManager.Instance.DeselectTower();
        if (towerController.currentLv == 1) UpgradeToLevel2();
        else UpgradeToLevel3();
    }

    public void UpgradeToLevel2() {
        // regenerate health
        towerController.currentHP = towerController.hp;
        towerController.UpdateHealthBar();

        // enhance the tower stats
        towerController.price += Mathf.RoundToInt(towerController.price * 0.8f);
        towerController.damage += 2;
        towerController.hp += 20;
        towerController.range += 0.5f;
        towerController.GetComponent<CircleCollider2D>().radius = towerController.range;
        towerController.DrawTowerRange(1);
        towerController.attackCooldown -= 0.5f;
        towerController.maintenanceEnergyPerSecond += 1;

        // update the tower detail
        towerDetail.GetTowerDetail();

        // update other attributes
        towerController.currentLv = 2;
        towerController.energyText.text = " -" + towerController.maintenanceEnergyPerSecond.ToString();
        upgradeCostText.text = towerController.upgradeCostToLv3.ToString();
        levelIndicator.text = "Lv.2";

        BaseManagement.Instance.coins -= towerController.upgradeCostToLv2;
    }

    public void UpgradeToLevel3()
    {
        // regenerate health
        towerController.currentHP = towerController.hp;
        towerController.UpdateHealthBar();

        // enhance the tower stats
        towerController.price += Mathf.RoundToInt(towerController.price * 0.4f);
        towerController.damage += 3;
        towerController.hp += 30;
        towerController.range += 0.5f;
        towerController.GetComponent<CircleCollider2D>().radius = towerController.range;
        towerController.DrawTowerRange(1);
        towerController.attackCooldown -= 0.5f;
        towerController.maintenanceEnergyPerSecond += 1;

        // update the tower detail
        towerDetail.GetTowerDetail();

        // update other attributes
        towerController.currentLv = 3;
        towerController.energyText.text = " -" + towerController.maintenanceEnergyPerSecond.ToString();
        levelIndicator.text = "Lv.3";
        isMaxLv = true;
        upgradeCostText.text = "MAX LV";
        DisplayUpgradeCost();

        BaseManagement.Instance.coins -= towerController.upgradeCostToLv3;
    }
}
