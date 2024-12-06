using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManagement : MonoBehaviour
{
    public static EnemyManagement Instance { get; set; }

    public List<GameObject> allNormalEnemiesOnRoute1 = new();
    public List<GameObject> allNormalEnemiesOnRoute2 = new();
    public List<GameObject> enemies = new(); // List of all enemies we can encounter
    public List<int> enemyPrice = new(); // List of corresponding cost of every enemies in list 'enemies'
    public GameObject towerBreakerPrefab;

    public Vector3[] waypoints_1 = {
        new(20.3f, 6f, 0), new(20.3f, 9.6f, 0), new(12.2f, 9.6f, 0), 
        new(12.2f, 5.4f, 0), new(5.2f, 5.4f, 0), new(5.2f, 8.6f, 0),
        new(-12.7f, 8.6f, 0), new(-12.7f, 1.4f, 0), new(-15.3f, 1.4f, 0),
        new(-18, 3.5f, 0), new(-23.4f, 3.5f, 0)
    };

    public Vector3[] waypoints_2 = {
        new(21.6f, -1.5f, 0), new(19.2f, 0.3f, 0), new(12.3f, 0.3f, 0),
        new(12.3f, -1.6f, 0), new(14f, -3.8f, 0), new(17.2f, -3.8f, 0),
        new(17.2f, -6.2f, 0), new(14.7f, -8.6f, 0), new(4.8f, -8.6f, 0),
        new(2.2f, -7.2f, 0), new(2.2f, -3.5f, 0), new(-2.7f, -3.5f, 0),
        new(-2.7f, 1.5f, 0), new(-9.75f, 1.5f, 0),new(-9.75f, -9.5f, 0),
        new(-12.7f, -9.5f, 0), new(-12.7f, -7.5f, 0), new(-19.75f, -7.5f, 0),
        new(-19.75f, -1.6f, 0), new(-23.4f, -1.6f, 0)
    };

    private Coroutine spawnTowerBreakerCoroutine;

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

    private void Start()
    {
        StartCoroutine(SpawnEnemy());
        SpotManager.Instance.OnTowerCountChanged += SpawnTowerBreakerWhenTowerExists;
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(Random.Range(4, 8));
        while (true)
        {
            // spawn 1-3 enemies every 10 seconds
            for (int enemyCount = Random.Range(1, 4); enemyCount > 0; enemyCount--)
            {
                int route = Random.Range(1, 3);
                if (route == 1)
                {
                    SpawnEnemyOnRoute1(0);
                }
                else
                {
                    SpawnEnemyOnRoute2(0);
                }
            }
            yield return new WaitForSeconds(10);
        }
    }

    public void SpawnEnemyOnRoute1(int enemyType)
    {
        if (EnemyBaseManagement.Instance.souls < enemyPrice[enemyType]) return;
        Vector3 spawnPosition = new(23, 3.3f, 0);
        Quaternion spawnRotation = Quaternion.identity;

        GameObject spawnedEnemy = Instantiate(enemies[enemyType], spawnPosition, spawnRotation);
        EnemyBaseManagement.Instance.souls -= enemyPrice[enemyType];

        EnemyMovement enemyMovemet = spawnedEnemy.GetComponent<EnemyMovement>();
        enemyMovemet.allNormalEnemiesOnRoute = allNormalEnemiesOnRoute1;
        enemyMovemet.waypoints = waypoints_1;

        allNormalEnemiesOnRoute1.Add(spawnedEnemy);
    }
    
    public void SpawnEnemyOnRoute2(int enemyType)
    {
        if (EnemyBaseManagement.Instance.souls < enemyPrice[enemyType]) return;
        Vector3 spawnPosition = new(23, -1.5f, 0);
        Quaternion spawnRotation = Quaternion.identity;

        GameObject spawnedEnemy = Instantiate(enemies[enemyType], spawnPosition, spawnRotation);
        EnemyBaseManagement.Instance.souls -= enemyPrice[enemyType];

        EnemyMovement enemyMovement = spawnedEnemy.GetComponent<EnemyMovement>();
        enemyMovement.allNormalEnemiesOnRoute = allNormalEnemiesOnRoute2;
        enemyMovement.waypoints = waypoints_2;

        allNormalEnemiesOnRoute2.Add(spawnedEnemy);
    }

    private void SpawnTowerBreakerWhenTowerExists(int towerCount)
    {
        if (towerCount == 1)
        {
            spawnTowerBreakerCoroutine = StartCoroutine(SpawnTowerBreaker());
        }
        else if (towerCount == 0)
        {
            StopCoroutine(spawnTowerBreakerCoroutine);
        }
    }

    private IEnumerator SpawnTowerBreaker()
    {
        yield return new WaitForSeconds(3);
        while (true)
        {
            GameObject towerBreaker1 = Instantiate(towerBreakerPrefab, waypoints_1[0], Quaternion.identity);
            towerBreaker1.GetComponent<TowerBreakerMovement>().waypoints = waypoints_1;

            GameObject towerBreaker2 = Instantiate(towerBreakerPrefab, waypoints_2[0], Quaternion.identity);
            towerBreaker2.GetComponent<TowerBreakerMovement>().waypoints = waypoints_2;

            yield return new WaitForSeconds(20);
        }
    }
}
