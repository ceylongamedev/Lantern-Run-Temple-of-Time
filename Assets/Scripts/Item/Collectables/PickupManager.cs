using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    [Header("Pickup Prefabs")]
    public List<GameObject> pickupPrefabs;

    [Header("Spawn Points")]
    public List<Transform> spawnPoints;

    private List<GameObject> spawnedPickups = new List<GameObject>();

    private void Start()
    {
        SpawnPickups();
    }


    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            ClearPickups();
            SpawnPickups();
        }
    }

    public void SpawnPickups()
    {
        ClearPickups();

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (pickupPrefabs.Count == 0) return;

            GameObject prefab =
                pickupPrefabs[Random.Range(0, pickupPrefabs.Count)];

            GameObject pickup = Instantiate(
                prefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            spawnedPickups.Add(pickup);
        }
    }

    void ClearPickups()
    {
        foreach (GameObject pickup in spawnedPickups)
        {
            if (pickup != null)
                Destroy(pickup);
        }

        spawnedPickups.Clear();
    }
}
