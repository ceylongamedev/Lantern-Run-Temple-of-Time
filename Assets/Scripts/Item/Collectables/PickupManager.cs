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

    [Header("Obstacle")]
    public List<GameObject> obstaclePrefabs;
    public List<Transform> obstacleSpawnPoints;
    private GameObject spawnedObstacle;

    private void OnEnable()
    {
        SpawnCoins();
        SpawnLantern();
        SpawnObstacle();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.C))
        {
            ClearCoins();
            ClearLantern();
            ClearObstacle();

            SpawnCoins();
            SpawnLantern();
            SpawnObstacle();
        }
    }

    public void SpawnCoins()
    {
        ClearCoins();

        if (coinPrefabs.Count == 0) return;

        foreach (Transform spawnPoint in coinSpawnPoints)
        {
            GameObject prefab =
                coinPrefabs[Random.Range(0, coinPrefabs.Count)];

            GameObject coin = Instantiate(
                prefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            coin.transform.SetParent(spawnPoint);
            coinSpawnedPickups.Add(coin);
        }
    }

    public void ClearCoins()
    {
        foreach (GameObject coin in coinSpawnedPickups)
        {
            if (coin != null)
                Destroy(coin);
        }
        coinSpawnedPickups.Clear();
    }

    public void SpawnLantern()
    {
        ClearLantern();

        if (lanternPrefabs.Count == 0 || lanternSpawnPoints.Count == 0) return;

        Transform randomPoint =
            lanternSpawnPoints[Random.Range(0, lanternSpawnPoints.Count)];

        GameObject prefab =
            lanternPrefabs[Random.Range(0, lanternPrefabs.Count)];

        spawnedLantern = Instantiate(
            prefab,
            randomPoint.position,
            randomPoint.rotation
        );

        spawnedLantern.transform.SetParent(randomPoint);
    }

    public void ClearLantern()
    {
        if (spawnedLantern != null)
            Destroy(spawnedLantern);
    }

    public void SpawnObstacle()
    {
        ClearObstacle();

        if (obstaclePrefabs.Count == 0 || obstacleSpawnPoints.Count == 0) return;

        Transform randomPoint =
            obstacleSpawnPoints[Random.Range(0, obstacleSpawnPoints.Count)];

        GameObject prefab =
            obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];

        spawnedObstacle = Instantiate(
            prefab,
            randomPoint.position,
            randomPoint.rotation
        );

        spawnedObstacle.transform.SetParent(randomPoint);
    }

    public void ClearObstacle()
    {
        if (spawnedObstacle != null)
            Destroy(spawnedObstacle);
    }
}
