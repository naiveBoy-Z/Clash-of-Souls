using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildTower : MonoBehaviour
{
    public GameObject towerPrefab;

    private void Start()
    {
        
    }

    public void Build()
    {
        Vector3 spawnPosition = transform.parent.position;
        Quaternion spawnRotation = Quaternion.identity;

        GameObject tower = Instantiate(towerPrefab, spawnPosition, spawnRotation);
        BaseManagement.Instance.coins -= tower.GetComponent<TowerController>().price;
    }
}
