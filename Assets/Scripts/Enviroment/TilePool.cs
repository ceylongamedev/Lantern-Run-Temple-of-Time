using System.Collections.Generic;
using UnityEngine;

public class TilePool
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    private GameObject prefab;
    private Transform parent;

    public TilePool(GameObject prefab, int initialSize, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;

        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = GameObject.Instantiate(prefab, parent);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (pool.Count == 0)
        {
            GameObject obj = GameObject.Instantiate(prefab, parent);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        GameObject tile = pool.Dequeue();
        tile.SetActive(true);
        return tile;
    }

    public void Return(GameObject tile)
    {
        tile.SetActive(false);
        pool.Enqueue(tile);
    }
}
