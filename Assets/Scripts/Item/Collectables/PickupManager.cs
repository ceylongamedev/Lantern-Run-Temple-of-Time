using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    [Header("Coin")]
    public List<GameObject> coinPrefabs;
    public List<Transform> coinSpawnPoints;
    private List<GameObject> coinSpawnedPickups = new List<GameObject>();

    [Header("Lantern")]
    public List<GameObject> lanternPrefabs;
    public List<Transform> lanternSpawnPoints;
    private GameObject spawnedLantern;

    [Header("Obstacle 1")]
    public List<GameObject> obstaclePrefabs;
    public List<Transform> obstacleSpawnPoints;
    private GameObject spawnedObstacle;

    [Header("Obstacle 2")]
    public List<GameObject> obstacle2Prefabs;
    public List<Transform> obstacle2SpawnPoints;
    private GameObject spawnedObstacle2;

    [Header("Power ups")]
    public List<GameObject> powerupsPrefabs;
    public List<Transform> powerupSpawnPoints;
    private GameObject spawnedPowerup;

    [Header("Objects Spawn Settings")]
    private float spawnDelay = 5f;
    private bool hasSpawnedInitially = false;

    private void OnEnable()
    {
        if (!hasSpawnedInitially)
        {
            StartCoroutine(DelayedInitialSpawn());
        }
        else
        {
            SpawnCoins();
            SpawnLantern();
            SpawnObstacle();
            SpawnObstacle2();
            SpawnPowerups();   // NEW
        }
    }

    private IEnumerator DelayedInitialSpawn()
    {
        yield return new WaitForSeconds(spawnDelay);
        hasSpawnedInitially = true;

        SpawnCoins();
        SpawnLantern();
        SpawnObstacle();
        SpawnObstacle2();
        SpawnPowerups();  
    }

    #region Coins
    public void SpawnCoins()
    {
        ClearCoins();

        if (coinPrefabs.Count == 0) return;

        foreach (Transform spawnPoint in coinSpawnPoints)
        {
            GameObject prefab = coinPrefabs[Random.Range(0, coinPrefabs.Count)];
            GameObject coin = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            coin.transform.SetParent(spawnPoint);
            coinSpawnedPickups.Add(coin);
        }
    }

    public void ClearCoins()
    {
        foreach (GameObject coin in coinSpawnedPickups)
        {
            if (coin != null) Destroy(coin);
        }
        coinSpawnedPickups.Clear();
    }
    #endregion

    #region Lantern
    public void SpawnLantern()
    {
        ClearLantern();

        if (lanternPrefabs.Count == 0 || lanternSpawnPoints.Count == 0) return;

        Transform randomPoint = lanternSpawnPoints[Random.Range(0, lanternSpawnPoints.Count)];
        GameObject prefab = lanternPrefabs[Random.Range(0, lanternPrefabs.Count)];

        spawnedLantern = Instantiate(prefab, randomPoint.position, randomPoint.rotation);
        spawnedLantern.transform.SetParent(randomPoint);
    }

    public void ClearLantern()
    {
        if (spawnedLantern != null) Destroy(spawnedLantern);
    }
    #endregion

    #region Obstacle 1
    public void SpawnObstacle()
    {
        ClearObstacle();

        if (obstaclePrefabs.Count == 0 || obstacleSpawnPoints.Count == 0) return;

        Transform randomPoint = obstacleSpawnPoints[Random.Range(0, obstacleSpawnPoints.Count)];
        GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];

        spawnedObstacle = Instantiate(prefab, randomPoint.position, randomPoint.rotation);
        spawnedObstacle.transform.SetParent(randomPoint);
    }

    public void ClearObstacle()
    {
        if (spawnedObstacle != null) Destroy(spawnedObstacle);
    }
    #endregion

    #region Obstacle 2
    public void SpawnObstacle2()
    {
        ClearObstacle2();

        if (obstacle2Prefabs.Count == 0 || obstacle2SpawnPoints.Count == 0) return;

        Transform randomPoint = obstacle2SpawnPoints[Random.Range(0, obstacle2SpawnPoints.Count)];
        GameObject prefab = obstacle2Prefabs[Random.Range(0, obstacle2Prefabs.Count)];

        spawnedObstacle2 = Instantiate(prefab, randomPoint.position, randomPoint.rotation);
        spawnedObstacle2.transform.SetParent(randomPoint);
    }

    public void ClearObstacle2()
    {
        if (spawnedObstacle2 != null) Destroy(spawnedObstacle2);
    }
    #endregion

    #region Obstacle 3
    public void SpawnPowerups()
    {
        ClearObstacle3();

        if (powerupsPrefabs.Count == 0 || powerupSpawnPoints.Count == 0) return;

        Transform randomPoint = powerupSpawnPoints[Random.Range(0, powerupSpawnPoints.Count)];
        GameObject prefab = powerupsPrefabs[Random.Range(0, powerupsPrefabs.Count)];

        spawnedPowerup = Instantiate(prefab, randomPoint.position, randomPoint.rotation);
        spawnedPowerup.transform.SetParent(randomPoint);
    }

    public void ClearObstacle3()
    {
        if (spawnedPowerup != null) Destroy(spawnedPowerup);
    }
    #endregion
}
