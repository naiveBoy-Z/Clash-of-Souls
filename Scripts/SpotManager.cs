using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpotManager : MonoBehaviour
{
    public static SpotManager Instance { get; set; }

    public SelectSpotEffect selectedSpot;
    private LayerMask layerMask;

    // create event
    public event Action<int> OnTowerCountChanged;
    public int towerCount = 0;

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
        layerMask = LayerMask.GetMask("Spot", "Building Selection");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, layerMask);
            if (hit.collider == null)
            {
                DeselectSpot(selectedSpot);
            }

            // if clicking on building spot (at "Spot" layer):
            else if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "Spot")
            {
                SelectSpot(hit.collider.GetComponent<SelectSpotEffect>());
            }

            // if clicking on building selection (at "Building Selection" layer):
            else
            {
                BuildTower(hit.collider.gameObject.GetComponent<BuildTower>());
            }
        }
    }

    public void SelectSpot(SelectSpotEffect selectSpotEffect)
    {
        if (selectSpotEffect != selectedSpot)
        {
            DeselectSpot(selectedSpot); // deselect current spot

            selectedSpot = selectSpotEffect;

            // turn on the select effect for selected building spot
            selectedSpot.isSelected = true;

            // create building selections
            selectedSpot.childObject.SetActive(true);
        }
        
    }

    public void DeselectSpot(SelectSpotEffect selectSpotEffect)
    {
        if (selectSpotEffect != null)
        {
            // destroy building selections
            selectedSpot.childObject.SetActive(false);

            // turn off the select effect for current building spot
            selectedSpot.isSelected = false;
            selectedSpot.spriteRenderer.color = Color.white;

            selectedSpot = null;
        }
    }

    public void BuildTower(BuildTower tower)
    {
        Destroy(selectedSpot.gameObject);
        DeselectSpot(selectedSpot);
        tower.Build();
        towerCount++;
        OnTowerCountChanged?.Invoke(towerCount);
    }

    public void SellTower(TowerController towerController)
    {
        Destroy(towerController.gameObject);
        towerCount--;
        OnTowerCountChanged?.Invoke(towerCount);
    }
}
