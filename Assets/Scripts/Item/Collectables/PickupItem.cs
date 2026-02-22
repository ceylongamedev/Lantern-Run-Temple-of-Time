using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickupItem : MonoBehaviour
{
    [Header("Detection")]
    public string playerTag = "Player";

    [Header("Pickup Values")]
    public int scoreValue = 10;

    [Header("Optional Effects")]
    public GameObject pickupEffect;

    private PointLightController[] pointLightControllers;

    public enum ItemType
    {
        Obstacle,
        Coin,
        Lantern
    };

    public ItemType type;

    private void Start()
    {
        pointLightControllers = FindObjectsByType<PointLightController>(FindObjectsSortMode.None);
    }


    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;


        switch (type)
        {
            case ItemType.Coin:
                break;
            case ItemType.Lantern:
                foreach (PointLightController pointLightController in pointLightControllers)
                {
                    pointLightController.IncreaseIntensity();
                }
                break;
            case ItemType.Obstacle:
                break;
        }
        
        Destroy(gameObject);
    }
}
