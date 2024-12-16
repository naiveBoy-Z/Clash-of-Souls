using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitManagement : MonoBehaviour
{
    public static UnitManagement Instance { get; set; }

    public List<GameObject> allNormalUnitsOnRoute1 = new();
    public List<GameObject> allNormalUnitsOnRoute2 = new();
    public List<GameObject> units = new(); // List of all units we can spawn
    public List<int> unitPrice = new(); // List of corresponding cost of every unit in list 'units'
    public List<GameObject> unitOnclickedBtn = new();
    public List<GameObject> unitIcons = new();
    public List<GameObject> deployFlags = new();
    public GameObject deployFlag;
    public int selectedUnitIndex = -1;

    public Vector3[] waypoints_1 = {
        new(-18, 3.5f, 0), new(-15.3f, 1.4f, 0), new(-12.7f, 1.4f, 0),
        new(-12.7f, 8.6f, 0), new(5.2f, 8.6f, 0), new(5.2f, 5.4f, 0),
        new(12.2f, 5.4f, 0), new(12.2f, 9.6f, 0), new(20.3f, 9.6f, 0),
        new(20.3f, 6f, 0), new(23, 3.3f, 0)
    };

    public Vector3[] waypoints_2 = {
        new(-18, -1.6f, 0), new(-19.75f, -7.5f, 0), new(-12.7f, -7.5f, 0),
        new(-12.7f, -9.5f, 0), new(-9.75f, -9.5f, 0), new(-9.75f, 1.5f, 0),
        new(-2.7f, 1.5f, 0), new(-2.7f, -3.5f, 0), new(2.2f, -3.5f, 0),
        new(2.2f, -7.2f, 0), new(4.8f, -8.6f, 0), new(14.7f, -8.6f, 0),
        new(17.2f, -6.2f, 0), new(17.2f, -3.8f, 0), new(14f, -3.8f, 0),
        new(12.3f, -1.6f, 0), new(12.3f, 0.3f, 0), new(19.2f, 0.3f, 0),
        new(21.6f, -1.5f, 0), new(23, -1.5f, 0)
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
        } else
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);
            if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("UnitSpawner"))
            {
                DeselectUnitSpawner();
            }
        }
    }

    public void SpawnUnit(int unitIndex)
    {
        if (selectedUnitIndex != unitIndex)
        {
            DeselectUnitSpawner();
        }
        SelectUnitSpawner(unitIndex);
    }

    private void SelectUnitSpawner(int unitIndex)
    {
        selectedUnitIndex = unitIndex;

        deployFlag.SetActive(true);

        unitOnclickedBtn[selectedUnitIndex].SetActive(true); // display other button's background
        unitIcons[selectedUnitIndex].transform.localScale = new(1.2f, 1.2f, 1); //scale icon
    }

    private void DeselectUnitSpawner()
    {
        if (selectedUnitIndex != -1)
        {
            deployFlag.SetActive(false);

            unitOnclickedBtn[selectedUnitIndex].SetActive(false); // display original button's background
            unitIcons[selectedUnitIndex].transform.localScale = new(1, 1, 1); //scale icon

            selectedUnitIndex = -1;
        }
    }

    public void DeployUnitLine1()
    {
        if (BaseManagement.Instance.souls < unitPrice[selectedUnitIndex]) return;
        Vector3 spawnPosition = new(-23.45f, 3.5f, 0);
        Quaternion spawnRotation = Quaternion.identity;

        GameObject deployedUnit = Instantiate(units[selectedUnitIndex], spawnPosition, spawnRotation);
        BaseManagement.Instance.souls -= unitPrice[selectedUnitIndex];

        UnitMovement unitMovement = deployedUnit.GetComponent<UnitMovement>();
        unitMovement.allNormalUnitsOnRoute = allNormalUnitsOnRoute1;
        unitMovement.waypoints = waypoints_1;

        allNormalUnitsOnRoute1.Add(deployedUnit);
    }
    public void DeployUnitLine2()
    {
        if (BaseManagement.Instance.souls < unitPrice[selectedUnitIndex]) return;
        Vector3 spawnPosition = new(-23.45f, -1.6f, 0);
        Quaternion spawnRotation = Quaternion.identity;

        GameObject deployedUnit = Instantiate(units[selectedUnitIndex], spawnPosition, spawnRotation);
        BaseManagement.Instance.souls -= unitPrice[selectedUnitIndex];

        UnitMovement unitMovement = deployedUnit.GetComponent<UnitMovement>();
        unitMovement.waypoints = waypoints_2;
        unitMovement.allNormalUnitsOnRoute = allNormalUnitsOnRoute2;

        allNormalUnitsOnRoute2.Add(deployedUnit);
    }
}

