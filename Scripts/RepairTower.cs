using TMPro;
using UnityEngine;

public class RepairTower : MonoBehaviour
{
    private TowerController towerController;
    // used to access the selected tower's hp, currentHP, repairCost

    public LayerMask layerMask;
    public GameObject repairIcon;
    public GameObject hpAmountInRepairSelection;
    public GameObject repairCostTextObject;
    public TextMeshProUGUI repairCostText;
    public bool isManipulating = false;


    void Start()
    {
        towerController = transform.parent.parent.GetComponent<TowerController>();
        layerMask = LayerMask.GetMask("Tower Selection");
    }

    void Update()
    {
        repairCostText.text = towerController.repairCost.ToString();

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);
            if (hit.collider != null && hit.collider.gameObject.name == "Repair Selection")
            {
                if (!isManipulating) isManipulating = true;
                else
                {
                    Repair();
                }
            }
        }
    }

    public void DisplayRepairCost()
    {
        repairIcon.SetActive(false);
        hpAmountInRepairSelection.SetActive(false);
        repairCostTextObject.SetActive(true);
    }

    private void OnDisable()
    {
        isManipulating = false;

        repairIcon.SetActive(true);
        hpAmountInRepairSelection.SetActive(true);
        repairCostTextObject.SetActive(false);
    }

    public void Repair()
    {
        TowerManager.Instance.DeselectTower();
        BaseManagement.Instance.coins -= towerController.repairCost;
        towerController.currentHP = towerController.hp;
        towerController.UpdateHealthBar();
    }
}
