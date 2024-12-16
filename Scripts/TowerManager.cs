using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(Instance);
    }

    public SelectTowerEffect selectedTower;
    private LayerMask layerMask;


    void Start()
    {
        layerMask = LayerMask.GetMask("Tower", "Tower Selection");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.RaycastAll(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);
            if (hits.Length == 0)
            {
                DeselectTower();
            }

            else
            {
                if (hits.Length == 1)
                {
                    if (LayerMask.LayerToName(hits[0].collider.gameObject.layer) == "Tower") DeselectTower();
                }
                foreach (var hit in hits)
                {
                    if (!hit.collider.isTrigger && LayerMask.LayerToName(hit.collider.gameObject.layer) == "Tower")
                    {
                        SelectTower(hit.collider.GetComponent<SelectTowerEffect>());
                    }
                    else if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "Tower Selection")
                    {
                        if (hit.collider.gameObject.name == "Repair Selection")
                        {
                            RepairTower(hit.collider.gameObject);
                        }
                        else if (hit.collider.gameObject.name == "Upgrade Selection")
                        {
                            UpgradeTower(hit.collider.gameObject);
                        }
                        else if (hit.collider.gameObject.name == "Detail Selection")
                        {
                            ShowTowerDetail(hit.collider.gameObject);
                        }
                        else
                        {
                            SellTower(hit.collider.gameObject);
                        }
                    }
                }
            }
        }
    }

    public void SelectTower(SelectTowerEffect newSelectedTower)
    {
        if (newSelectedTower != selectedTower)
        {
            DeselectTower(); // deselect current tower

            selectedTower = newSelectedTower;

            // activate the selection effect for the selected tower
            selectedTower.isSelected = true;

            // create the tower controller
            selectedTower.towerController.SetActive(true);

            // display the tower range
            selectedTower.GetComponent<LineRenderer>().enabled = true;
        }

    }

    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            // destroy building selections
            selectedTower.towerController.SetActive(false);

            // turn off the select effect for current building spot
            selectedTower.isSelected = false;
            selectedTower.spriteRenderer.color = Color.white;

            // hide the tower range
            selectedTower.GetComponent<TowerController>().DrawTowerRange(1);
            selectedTower.GetComponent<LineRenderer>().enabled = false;

            selectedTower = null;
        }
    }

    public void RepairTower(GameObject repairSelection)
    {
        repairSelection.GetComponent<RepairTower>().DisplayRepairCost();
    }

    public void UpgradeTower(GameObject upgradeSelection)
    {
        upgradeSelection.GetComponent<UpgradeTower>().DisplayUpgradeCost();
        int numberOfRanges = upgradeSelection.GetComponent<UpgradeTower>().isMaxLv ? 1 : 2;
        upgradeSelection.transform.parent.parent.gameObject.GetComponent<TowerController>().DrawTowerRange(numberOfRanges);
    }

    public void ShowTowerDetail(GameObject detailSelection)
    {
        detailSelection.GetComponent<TowerDetail>().ShowTowerDetail();
    }

    public void SellTower(GameObject sellSelection)
    {
        sellSelection.GetComponent<SellTower>().DisplaySellCost();
    }
}
