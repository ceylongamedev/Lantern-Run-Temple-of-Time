using UnityEngine;

public class MahasonaDetact : MonoBehaviour
{
    [Header("Settings")]
    public float range = 5f;
    public string playerTag = "Player";

    private Transform player;
    private bool hasTriggered = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("Player not found! Make sure tag is correct.");
    }

    void Update()
    {
        if (player == null || hasTriggered) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= range)
        {
            hasTriggered = true;
            OnPlayerEnterRange();
        }
    }

    void OnPlayerEnterRange()
    {
        Debug.Log("Player entered range!");
        DestroyObject();
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
