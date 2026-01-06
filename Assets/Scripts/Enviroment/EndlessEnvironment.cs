using System.Collections.Generic;
using UnityEngine;

public class EndlessEnvironment : MonoBehaviour
{
    public Transform cameraTransform;
    public float runSpeed = 10f;

    [Header("Spawn Settings")]
    public float spawnAheadDistance = 50f;
    public float despawnBehindDistance = 60f;

    public List<GameObject> tilePrefabs;
    public int poolSizePerTile = 5;

    private Dictionary<GameObject, TilePool> pools = new Dictionary<GameObject, TilePool>();
    private List<GameObject> activeTiles = new List<GameObject>();

    void Awake()
    {
        foreach (GameObject prefab in tilePrefabs)
        {
            pools[prefab] = new TilePool(prefab, poolSizePerTile, transform);
        }
    }

    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            SpawnTile();
        }
    }

    void Update()
    {
        MoveTiles();
        SpawnIfNeeded();
        DespawnTiles();
    }

    void MoveTiles()
    {
        foreach (GameObject tile in activeTiles)
        {
            tile.transform.Translate(Vector3.back * runSpeed * Time.deltaTime);
        }
    }

    void SpawnIfNeeded()
    {
        if (activeTiles.Count == 0) return;

        RunnerTile lastTile = activeTiles[activeTiles.Count - 1].GetComponent<RunnerTile>();

        if (lastTile.EndZ < cameraTransform.position.z + spawnAheadDistance)
        {
            SpawnTile();
        }
    }

    void SpawnTile()
    {
        GameObject prefab = tilePrefabs[Random.Range(0, tilePrefabs.Count)];
        GameObject tile = pools[prefab].Get();

        float spawnZ = 0f;

        if (activeTiles.Count > 0)
        {
            RunnerTile lastTile = activeTiles[activeTiles.Count - 1].GetComponent<RunnerTile>();
            spawnZ = lastTile.EndZ;
        }

        tile.transform.position = new Vector3(0f, 0f, spawnZ);
        tile.transform.rotation = Quaternion.identity;

        tile.GetComponent<TileIdentity>().prefab = prefab;

        activeTiles.Add(tile);
    }


    void DespawnTiles()
    {
        if (activeTiles.Count == 0) return;

        GameObject firstTile = activeTiles[0];

        if (firstTile.transform.position.z < cameraTransform.position.z - despawnBehindDistance)
        {
            activeTiles.RemoveAt(0);

            TileIdentity id = firstTile.GetComponent<TileIdentity>();
            pools[id.prefab].Return(firstTile); 
        }
    }

}
