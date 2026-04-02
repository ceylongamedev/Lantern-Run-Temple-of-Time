using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public float minSpawnDistance = 6f;
    public int maxSpawnAttempts = 10;

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

    [Header("Spawn Settings")]
    public float spawnDelay = 5f;
    private bool hasSpawnedInitially = false;

    private void OnEnable()
    {
        if (!hasSpawnedInitially)
        {
            StartCoroutine(DelayedInitialSpawn());
        }
        else
        {
            SpawnAll();
        }
    }

    private void Start()
    {
        player = Object.FindAnyObjectByType<PlayerControler>().gameObject.transform;
    }

    private IEnumerator DelayedInitialSpawn()
    {
        yield return new WaitForSeconds(spawnDelay);
        hasSpawnedInitially = true;

        SpawnAll();
    }

    private void SpawnAll()
    {
        SpawnCoins();
        SpawnLantern();
        SpawnObstacle();
        SpawnObstacle2();
        SpawnPowerups();
    }

    // ================= SAFE SPAWN CORE =================

    Transform GetSafeSpawnPoint(List<Transform> spawnPoints)
    {
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];

            float distance = Vector3.Distance(player.position, point.position);

            if (distance >= minSpawnDistance)
            {
                return point;
            }
        }

        return null; // No safe position found
    }

    GameObject SpawnObject(List<GameObject> prefabs, List<Transform> spawnPoints, ref GameObject spawnedObj)
    {
        if (prefabs.Count == 0 || spawnPoints.Count == 0) return null;

        Transform point = GetSafeSpawnPoint(spawnPoints);
        if (point == null) return null;

        GameObject prefab = prefabs[Random.Range(0, prefabs.Count)];

        spawnedObj = Instantiate(prefab, point.position, point.rotation);
        spawnedObj.transform.SetParent(point);

        return spawnedObj;
    }

    void ClearObject(ref GameObject obj)
    {
        if (obj != null)
        {
            Destroy(obj);
            obj = null;
        }
    }

    // ================= COINS =================

    public void SpawnCoins()
    {
        ClearCoins();

        foreach (Transform spawnPoint in coinSpawnPoints)
        {
            if (Vector3.Distance(player.position, spawnPoint.position) < minSpawnDistance)
                continue;

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

    // ================= LANTERN =================

    public void SpawnLantern()
    {
        ClearObject(ref spawnedLantern);
        SpawnObject(lanternPrefabs, lanternSpawnPoints, ref spawnedLantern);
    }

    // ================= OBSTACLE 1 =================

    public void SpawnObstacle()
    {
        ClearObject(ref spawnedObstacle);
        SpawnObject(obstaclePrefabs, obstacleSpawnPoints, ref spawnedObstacle);
    }

    // ================= OBSTACLE 2 =================

    public void SpawnObstacle2()
    {
        ClearObject(ref spawnedObstacle2);
        SpawnObject(obstacle2Prefabs, obstacle2SpawnPoints, ref spawnedObstacle2);
    }

    // ================= POWERUPS =================

    public void SpawnPowerups()
    {
        ClearObject(ref spawnedPowerup);

        float chance = Random.value;
        //if (chance > 0.2f) return;

        SpawnObject(powerupsPrefabs, powerupSpawnPoints, ref spawnedPowerup);
    }
}